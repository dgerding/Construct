/*
    Copyright (C) 2008 Seeing Machines Ltd. All rights reserved.

    This file is part of the CoreData API.

    This file may be distributed under the terms of the SM Non-Commercial License
    Agreement appearing in the file LICENSE.TXT included in the packaging
    of this file.

    This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
    WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

    Further information about the CoreData API is available at:
    http://www.seeingmachines.com/

*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using sm.eod;
using sm.eod.io;

namespace logconverter
{
    class Program
    {
        static bool convertFile(string filepath)
        {
            //make output filename
            if (!Path.HasExtension(filepath))
            {
                Console.Error.WriteLine("Bad filename - must be of type .fll");
                return false;
            }
            string outputFile = Path.ChangeExtension(filepath, ".txt");
            try
            {
                sm.eod.utils.eodutils.ConvertLogToText(filepath, outputFile);
            }
            catch (sm.eod.EODError e)
            {
                Console.Error.WriteLine("{0}", e.Message);
                return false;
            }
            return true;
        }


        static void Main(string[] args)
        {
            const int EXIT_SUCCESS = 0;
            const int EXIT_FAILURE = 1;

            if (args.Length < 1)
            {
                Console.Error.WriteLine("Bad number of arguments\n"
                    + "Usage: logconverter filename(s) \n"
                    + "Reads faceLAB binary logfiles, and prints them in text form.\n"
                    + "Logfile must have file extension .fll\n");

                System.Environment.Exit(EXIT_FAILURE);
            }
            foreach (string filepath in args)
            {
                Console.Error.WriteLine("Converting file: {0} ...", filepath);
                if (convertFile(filepath))
                {
                    Console.WriteLine("done");
                }
                else
                {
                    Console.Error.WriteLine("Conversion failed!");
                }
            }
            System.Environment.Exit(EXIT_SUCCESS);
        }
    }
}

