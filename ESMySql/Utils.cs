﻿using System;
using System.Runtime.InteropServices;


namespace ESMySql
{
    class Utils
    {
        /// <summary>
        /// Print out a message only if the program is compiled for Debug.
        /// </summary>
        /// <param name="data">Data to print in console</param>
        static internal void DebugWrite(string data)
        {
#if DEBUG
            CitizenFX.Core.Debug.Write(data);
#endif
        }

        /// <summary>
        /// Print out a message only if the program is compiled for Debug.
        /// </summary>
        /// /// <param name="data">Data to print in console</param>
        /// /// <param name="args">Arugments to be formated into data</param>
        static internal void DebugWriteLine(string data, [Optional]object[] args)
        {
#if DEBUG
            if (args != null)
            {
                CitizenFX.Core.Debug.WriteLine(data, args);
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine(data);
            }
#endif
        }
        /// <summary>
        /// Print out a message only if the program is compiled for all release types.
        /// </summary>
        /// /// <param name="data">Data to print in console</param>
        static internal void ReleaseWrite(string data)
        {
            CitizenFX.Core.Debug.Write(data);
        }

        /// <summary>
        /// Print out a message only if the program is compiled for all release types.
        /// </summary>
        /// <param name="data">Data to print in console</param>
        /// <param name="args">Arugments to be formated into data</param>
        static internal void ReleaseWriteLine(string data, [Optional]object[] args)
        {
            if (args != null)
            {
                CitizenFX.Core.Debug.WriteLine(data, args);
            }
            else
            {
                CitizenFX.Core.Debug.WriteLine(data);
            }
        }

        /// <summary>
        /// Print out a message only if the program is compiled for all release types.
        /// </summary>
        ///<param name = "ex">Message for exception</param>
        static internal void ThrowException(Exception ex)
        {
            ReleaseWriteLine($"Exception thrown:\n" +
                $"{ex.Message}");
            throw (ex);
        }

    }
}
