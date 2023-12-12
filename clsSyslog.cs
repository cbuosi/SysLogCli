using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml;


public class clsSyslog
{

    //The BSD syslog Protocol
    //https://tools.ietf.org/html/rfc3164

    const string ARQUIVO_DATABASE_CONFIG = "Config.xml";

    private const int SysLogDefaultPort = 514;
    private const bool ASSINCRONO = true;

    internal static bool bInicializado = false;

    internal static UdpClient? _UdpClient = null;
    internal static int _facility;
    internal static string _hostName = "";
    internal static int _port = -1;

    public enum eSysLogFacility
    {
        KERN = 0,
        USER = 1,
        MAIL = 2,
        DAEMON = 3,
        AUTH = 4,
        SYSLOG = 5,
        LPR = 6,
        NEWS = 7,
        UUCP = 8,
        CRON = 9,
        AUTHPRIV = 10,
        FTP = 11,
        NTP = 12,
        SECURITY = 13,
        CONSOLE = 14,
        SOLARIS = 15,
        LOCAL0 = 16,
        LOCAL1 = 17,
        LOCAL2 = 18,
        LOCAL3 = 19,
        LOCAL4 = 20,
        LOCAL5 = 21,
        LOCAL6 = 22,
        LOCAL7 = 23
    }

    public enum eSysLogSeverity
    {
        EMERGENCY = 0,
        ALERT = 1,
        CRITICAL = 2,
        ERROR = 3,
        WARNING = 4,
        NOTICE = 5,
        INFORMATIONAL = 6,
        DEBUG = 7
    }


    public clsSyslog(eSysLogFacility facility)
    {
        try
        {
            if (string.IsNullOrEmpty(_hostName))
            {
                _hostName = ObterConfig("SYSLOG_SERVER");
            }

            if (int.TryParse(ObterConfig("SYSLOG_PORT"), out int parsedPort))
            {
                _port = parsedPort;
            }
            else
            {
                _port = SysLogDefaultPort;
            }

            _facility = (int)facility;

            _UdpClient = new UdpClient();

            bInicializado = true;
        }
        catch (Exception ex)
        {
            Erro(NomeMetodo("clsSyslog"), ex);
        }
    }

    public clsSyslog(string hostName,
                     int port,
                     eSysLogFacility facility = eSysLogFacility.USER)
    {
        try
        {
            _hostName = hostName;
            _port = port;
            _facility = (int)facility;

            _UdpClient = new UdpClient();

            bInicializado = true;
        }
        catch (Exception ex)
        {
            Erro(NomeMetodo("clsSyslog"), ex);
        }
    }

    public Boolean Send(eSysLogSeverity logLevel, string sender, string message, string pid = "")
    {
        try
        {
            string strEnvio = "";
            byte[] bytes;

            if (bInicializado == false)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_hostName) || _port == 0)
            {
                return false;
            }

            if (_UdpClient == null)
            {
                return false;
            }

            // PRI
            strEnvio = "<" + (_facility * 8 + (int)logLevel) + ">";

            // HEADER 
            if (pid == "")
            {
                strEnvio += ObterMesRFC3164(DateTime.Now.Month) + DateTime.Now.ToString(" dd HH:mm:ss ") + Dns.GetHostName() + "_" + ObterNomeUsuarioLogado() + " " + sender + ": " + message;
            }
            else
            {
                strEnvio += ObterMesRFC3164(DateTime.Now.Month) + DateTime.Now.ToString(" dd HH:mm:ss ") + Dns.GetHostName() + "_" + ObterNomeUsuarioLogado() + " " + sender + "[" + pid + "]: " + message;
            }

            Console.WriteLine(strEnvio);

            bytes = Encoding.Default.GetBytes(strEnvio);

            if (ASSINCRONO)
            {
                _UdpClient.BeginSend(bytes, bytes.Length, _hostName, _port, new AsyncCallback(AsyncCompleted), _UdpClient);
            }
            else
            {
                _UdpClient.Send(bytes, bytes.Length, _hostName, _port);
            }
            return true;
        }
        catch (Exception ex)
        {
            Erro(NomeMetodo("clsSyslog"), ex);
            return false;

        }
    }

    public static void AsyncCompleted(IAsyncResult result)
    {
        try
        {
            System.Diagnostics.Debug.Print("result.IsCompleted: " + result.IsCompleted);
        }
        catch (Exception ex)
        {
            Erro(NomeMetodo("clsSyslog"), ex);
        }
    }

    public static string ObterMesRFC3164(int Mes)
    {
        string[] strMeses = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        return strMeses[Mes - 1];
    }

    private static void Erro(string strMetodo, Exception ex)
    {
        System.Diagnostics.Debug.Print("Erro em: " + strMetodo + " : " + ex.Message);
    }

    public static string ObterConfig(string chave)
    {
        XmlDocument xmlConfig = new XmlDocument();
        string strPath;

        try
        {
            strPath = ""; // HttpContext.Current.Server.MapPath("~");

            xmlConfig = new XmlDocument();
            xmlConfig.Load(Path.Combine(strPath, ARQUIVO_DATABASE_CONFIG));

#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
            return xmlConfig.DocumentElement.SelectSingleNode("//Config").SelectSingleNode("//" + chave).Attributes["valor"].InnerText.ToString();
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.

            // Se estiver usando configurações de aplicativo (app settings) em vez de um arquivo XML
            // return System.Configuration.ConfigurationManager.AppSettings[chave];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro: " + ex.Message);
            return "";
        }
    }

    private static string NomeMetodo(string className)
    {
        // Implementar a lógica para obter o nome do método atual.
#pragma warning disable CS8602 // Desreferência de uma referência possivelmente nula.
        return className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name;
#pragma warning restore CS8602 // Desreferência de uma referência possivelmente nula.
    }

    private static string ObterNomeUsuarioLogado()
    {
        return Environment.UserName;
    }
}

