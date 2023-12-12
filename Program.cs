using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;
using System.Globalization;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using System.Web;
using System.Diagnostics;
using Microsoft.VisualBasic;

public class SysLogCli
{
    const string VERSAO = "2.00";

    public static int Main(String[] args)
    {
        Log("Inicio");
        int tot = 0;

        clsSyslog oSyslog;

        while (true)
        {

            //oSyslog = new clsSyslog(clsSyslog.eSysLogFacility.FTP);
            //oSyslog.Send(clsSyslog.eSysLogSeverity.ERROR, "PROGRAMA_TESTE", "ola, sou uma msg: " + tot.ToString());

            for (int i = 0; i <= 23; i++)
            {
                oSyslog = new clsSyslog((clsSyslog.eSysLogFacility)i);
                for (int j = 0; j <= 7; j++)
                {
                    tot += 1;
            
                    if (tot % 2 == 0)
                    {
                        oSyslog.Send((clsSyslog.eSysLogSeverity)j, "PROGRAMA_TESTE", "ola, sou uma msg: " + tot.ToString());
                    }
                    else
                    {
                        oSyslog.Send((clsSyslog.eSysLogSeverity)j, "PROGRAMA_TESTE", "ola, sou uma msg: " + tot.ToString(), tot.ToString());
                    }

                    //"<" + (_facility * 8 + (int)logLevel) + ">";
                   
                    //Console.ReadLine(); 

                }
            }
        }

        //oSyslog = new clsSyslog(clsSyslog.eSysLogFacility.USER);
        //oSyslog.Send(clsSyslog.eSysLogSeverity.DEBUG, "PROGRAMA_TESTE", "ola, sou uma msg: 1");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.EMERGENCY, "PROGRAMA_TESTE", "ola, sou uma msg 2");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.ALERT, "PROGRAMA_TESTE", "ola, sou uma msg3");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.CRITICAL, "PROGRAMA_TESTE", "ola, sou uma msg4");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.ERROR, "PROGRAMA_TESTE", "ola, sou uma msg5");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.WARNING, "PROGRAMA_TESTE", "ola, sou uma msg6");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.NOTICE, "PROGRAMA_TESTE", "ola, sou uma msg7");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.INFORMATIONAL, "PROGRAMA_TESTE", "ola, sou uma msg8");
        //oSyslog.Send(clsSyslog.eSysLogSeverity.DEBUG, "PROGRAMA_TESTE", "ola, sou uma msg9");


        //oSyslog.Equals

        return 0;
    }

    private static void Log(string _strLog)
    {
        System.Diagnostics.Debug.Print(_strLog);
        Console.WriteLine(_strLog);


    }

}