using System.Collections;
using System.Reflection;
using DeliverCom.Core.Exception.Impl;
using DeliverCom.Core.Helper;

#pragma warning disable CS1591

namespace DeliverCom.Core.Exception.Infrastructure
{
    public abstract class BaseException : System.Exception
    {
        private const string DefaultErrorCode = "UNKNOWN_ERROR";
        private static readonly object MessageFieldLock = new();
        private static FieldInfo _messageField;
        private string _message;

        protected BaseException(string message)
            : base(message)

        {
            Init(message, null);
        }

        public string ExceptionMessage => this.GetExceptionMessage();


        public virtual string Name => GetType().Name;
        private string ErrorCode { get; set; }


        #region Public Properties

        public override string Message =>
            string.IsNullOrEmpty((_message ?? string.Empty).Trim()) ? base.Message : _message;

        #endregion Public Properties

        #region Private Methods

        private void TrySetField(string fieldName, object parameters)
        {
            try
            {
                var prmFlags = BindingFlags.NonPublic | BindingFlags.Instance
                                                      | BindingFlags.SetField | BindingFlags.IgnoreCase;
                typeof(System.Exception).InvokeMember(fieldName, prmFlags, null, this, new[]
                    {
                        parameters
                    },
                    null, null, null);
            }
            catch (System.Exception)
            {
                //Ignored
            }
        }


        private static string GetExceptionMethod(System.Exception ex)
        {
            var mFlags = BindingFlags.Public |
                         BindingFlags.Instance | BindingFlags.InvokeMethod;

            var cInfo = typeof(System.Exception).GetMethod("GetExceptionMethodString", mFlags, null,
                CallingConventions.Any, Type.EmptyTypes, null);

            if (cInfo != null) return cInfo.Invoke(ex, Array.Empty<object>()) as string;

            return null;
        }

        public static T FromException<T>(System.Exception e) where T : BaseException
        {
            if (e == null) return null;

            var ex = Activator.CreateInstance<T>();

            ex.TrySetField("_stackTraceString", e.StackTrace);
            ex.TrySetField("_exceptionMethodString", GetExceptionMethod(e));
            ex.TrySetField("_data", e.Data.Count > 0 ? new Hashtable(e.Data) : null);
            ex.TrySetField("_innerException",
                e.InnerException is BaseException
                    ? e.InnerException
                    : FromException<UnknownException>(e.InnerException));
            ex.SetMessageToException(e.Message);

            ex.Source = e.Source;
            ex.HResult = e.HResult;
            ex.HelpLink = e.HelpLink;

            if (e is BaseException exception) ex.ErrorCode = exception.ErrorCode;


            return ex;
        }

        private static FieldInfo GetMessageMemberInfo()
        {
            lock (MessageFieldLock)
            {
                if (_messageField != null) return _messageField;
                var messageField = typeof(System.Exception).GetField("_message",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);

                Interlocked.Exchange(ref _messageField, messageField);

                return _messageField;
            }
        }

        private string GetParameterKey(string parameter)
        {
            return $"PRM_{parameter}";
        }

        private void Init(string message, Dictionary<string, string> args)
        {
            if (args != null)
                message = args.Aggregate(message,
                    (current, arg) => current.Replace(GetParameterKey(arg.Key), arg.Value));
            SetMessageToException(message);
        }

        private void SetMessageToException(string message)
        {
            if (TrySetMessage(message)) _message = (message ?? string.Empty).Trim();
        }

        private bool TrySetMessage(string message)
        {
            try
            {
                var fieldInfo = GetMessageMemberInfo();
                if (fieldInfo == null) return false;
                fieldInfo.SetValue(this, message);
                return true;
            }
            catch (System.Exception)
            {
                //Ignored
            }

            return false;
        }

        #endregion Private Methods
    }
}