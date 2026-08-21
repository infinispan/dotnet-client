using System;
using System.Collections.Generic;
using System.Text;

namespace Infinispan.Hotrod
{
    public class Result
    {
        public string Messge { get; set; }

        public ResultStatus Status { get; set; } = ResultStatus.None;

        public ResultType ResultType { get; internal set; }

        public bool IsError
        {
            get
            {
                return (this.ResultType == ResultType.DataError ||
                     this.ResultType == ResultType.Error
                     || this.ResultType == ResultType.NetError);
            }
        }
    }
    /// <summary>
    /// CommandResult represent all the info related to a command execution
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Results contains all the messages related to execution.
        /// </summary>
        public List<Result> Results = new List<Result>();
        /// <summary>
        /// IsError is true if the final status of the command is not executed due to error.
        /// </summary>
        internal bool IsError = false;
        /// <summary>
        /// ErrorMessage is the error related to the failure of the command.
        /// </summary>
        public string ErrorMessage;
    }
}
