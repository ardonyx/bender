﻿namespace Bender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class Options
    {
        public Options()
        {
            Okay = true;
        }

        public bool Okay { get; set; }

        public string Message { get; set; }

        public string SpecFile { get; set; }

        public string BinaryFile { get; set; }

        public bool PrintSpec { get; set; }

        public static implicit operator bool(Options o)
        {
            return o.Okay;
        }
        public override string ToString()
        {
            return Message;
        }
    }
    
    class ArgParser
    {       
        private IList<string> required_ = new List<string>() { "-f", "-b", "-p" };

        public ArgParser()
        {

        }

        public Options Parse(string[] args)
        {
            Options result = new Options();

            if (args.Length == 0)
            {
                result.Okay = false;
                result.Message = Usage();
                return result;
            }

            Action<string> nextCapture = null;
            var satisified = new List<string>();

            // Read input in pairs
            foreach(var str in args)
            {
                if (!result.Okay)
                {
                    return result;
                }
                else if (nextCapture != null)
                {
                    nextCapture(str);
                    nextCapture = null;
                }
                else
                {
                    switch (str)
                    {
                        case "-f":
                        case "--file":
                            nextCapture = (s) => result.SpecFile = s;
                            satisified.Add("-f");
                            break;
                        case "-b":
                        case "--binary":
                            nextCapture = (s) => result.BinaryFile = s;
                            satisified.Add("-b");
                            break;
                        case "-p":
                        case "--print-spec":
                            result.PrintSpec = true;
                            break;
                        default:
                            result.Okay = false;
                            result.Message = string.Format("Unknown argument: {0}", str);
                            break;
                    }
                }
            }

            if(satisified.Equals(required_))
            {
                result.Okay = false;
                result.Message = string.Format("Missing expected parameters: {0}",
                    string.Join(",", required_.Except(satisified)));
            }

            return result;
        }

        public string Usage()
        {
            return "Bender.exe -f /path/to/spec.yaml -b /path/to/your.bin";
        }
    }
}
