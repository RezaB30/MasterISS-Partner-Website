using NLog;
using RezaB.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Partner_WebSite_Scheduler
{
    public static class StreamExtensions
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);

            return base64;
        }


        public static string GetFileBase64StringValue(long taskNo, short attachmentType, string filename)
        {
            var fileManager = GetLocalFileManager();

            var validFileSetup = fileManager.DirectoryExists("CustomerOperations");
            if (validFileSetup.InternalException == null)
            {
                if (validFileSetup.Result == false)
                {
                    return null;
                }

                var enterSetupOperations = fileManager.EnterDirectoryPath("CustomerOperations");
                if (enterSetupOperations.InternalException == null)
                {
                    var validTaskFile = fileManager.DirectoryExists(taskNo.ToString());

                    if (validTaskFile.InternalException == null)
                    {
                        if (validTaskFile.Result == false)
                        {
                            return null;
                        }

                        var enterTaskFile = fileManager.EnterDirectoryPath(taskNo.ToString());
                        if (enterTaskFile.InternalException == null)
                        {
                            var validAttachmentType = fileManager.DirectoryExists(attachmentType.ToString());
                            if (validAttachmentType.InternalException == null)
                            {
                                if (validAttachmentType.Result == false)
                                {
                                    return null;
                                }

                                var enteredAtttachmentType = fileManager.EnterDirectoryPath(attachmentType.ToString());

                                if (enteredAtttachmentType.InternalException == null)
                                {
                                    var validFile = fileManager.FileExists(filename);
                                    if (validFile.InternalException == null)
                                    {
                                        if (validFile.Result == false)
                                        {
                                            LoggerError.Fatal($"File Not Found");
                                            return null;
                                        }
                                        var getFile = fileManager.GetFile(filename);

                                        if (getFile.InternalException == null)
                                        {
                                            var base64StringValue =ConvertToBase64(getFile.Result);

                                            return base64StringValue;
                                        }
                                        else
                                        {
                                            LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => getFile : Error Message : {getFile.InternalException.Message}");
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => validFile : Error Message : {validFile.InternalException.Message}");
                                        return null;
                                    }
                                }
                                else
                                {
                                    LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => validFile : Error Message : {enteredAtttachmentType.InternalException.Message}");
                                    return null;
                                }
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => validAttachmentFile : Error Message : {validAttachmentType.InternalException.Message}");
                                return null;
                            }
                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => enterTaskFile : Error Message : {enterTaskFile.InternalException.Message}");
                            return null;
                        }
                    }
                    else
                    {
                        LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => validTaskFile : Error Message : {validTaskFile.InternalException.Message}");
                        return null;
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => enterSetupOperations : Error Message : {enterSetupOperations.InternalException.Message}");
                    return null;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred GetCustomerAttachmentType => validFileSetup : Error Message : {validFileSetup.InternalException.Message}");
                return null;
            }
        }

        private static IFileManager GetLocalFileManager()
        {
            var rootFolder = Environment.GetEnvironmentVariable("RadiusR_Repo");

            IFileManager fileManager = new RezaB.Files.Local.LocalFileManager(rootFolder);

            return fileManager;
        }
    }
}
