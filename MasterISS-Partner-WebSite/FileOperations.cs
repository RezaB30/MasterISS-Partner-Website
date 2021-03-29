using NLog;
using RezaB.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MasterISS_Partner_WebSite
{
    public class FileOperations
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public bool SaveCustomerForm(Stream stream, int attachmentTypesEnum, string fileName, long taskNo)
        {
            var fileManager = GetLocalFileManager();
            var validFile = fileManager.DirectoryExists("CustomerOperations");
            if (validFile.InternalException == null)
            {
                if (validFile.Result == false)
                {
                    var createDirectory = fileManager.CreateDirectory("CustomerOperations");
                    if (createDirectory.InternalException != null)
                    {
                        LoggerError.Fatal($"An Error Occurred FileOperations=> createDirectory : Error Message : {createDirectory.InternalException.Message}");
                        return false;
                    }
                }

                var enterCustomerOperations = fileManager.EnterDirectoryPath("CustomerOperations");

                if (enterCustomerOperations.InternalException == null)
                {
                    var validTaskNo = fileManager.DirectoryExists(taskNo.ToString());

                    if (validTaskNo.InternalException != null)
                    {
                        LoggerError.Fatal($"An Error Occurred validTaskNo => Error Message : {validTaskNo.InternalException.Message}");
                        return false;
                    }
                    else
                    {
                        if (validTaskNo.Result == false)
                        {
                            var createDirectoryTask = fileManager.CreateDirectory(taskNo.ToString());
                            if (createDirectoryTask.InternalException != null)
                            {
                                LoggerError.Fatal($"An Error Occurred createDirectoryTask => Error Message : {createDirectoryTask.InternalException.Message}");
                                return false;
                            }
                        }

                        var enteredTask = fileManager.EnterDirectoryPath(taskNo.ToString());

                        if (enteredTask.InternalException == null)
                        {
                            var validForm = fileManager.DirectoryExists(attachmentTypesEnum.ToString());

                            if (validForm.InternalException == null)
                            {
                                if (validForm.Result == false)
                                {
                                    var createDirectoryForm = fileManager.CreateDirectory(attachmentTypesEnum.ToString());

                                    if (createDirectoryForm.InternalException != null)
                                    {
                                        LoggerError.Fatal($"An Error Occurred FileOperations=> createDirectoryForm: Error Message : {validForm.InternalException.Message}");
                                        return false;
                                    }
                                }

                                var enterAttachmentType = fileManager.EnterDirectoryPath(attachmentTypesEnum.ToString());

                                if (enterAttachmentType.InternalException == null)
                                {
                                    var saveFile = fileManager.SaveFile(fileName, stream, false);

                                    if (saveFile.InternalException == null)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        LoggerError.Fatal($"An Error Occurred FileOperations=> fileManager.SaveFile() : Error Message : {saveFile.InternalException.Message}");
                                        return false;
                                    }
                                }
                                else
                                {
                                    LoggerError.Fatal($"An Error Occurred FileOperations=> enterAttachmentType: Error Message : {enterAttachmentType.InternalException.Message}");
                                    return false;
                                }
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred FileOperations=> validForm.DirectoryExists() : Error Message : {validForm.InternalException.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred FileOperations=> enteredTask : Error Message : {enteredTask.InternalException.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred FileOperations=> fileManager.EnterDirectoryPath() : Error Message : {enterCustomerOperations.InternalException.Message}");
                    return false;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred FileOperations=> fileManager.DirectoryExists() : Error Message : {validFile.InternalException.Message}");
                return false;
            }
        }

        public bool SaveSetupFile(Stream stream, string fileName, long taskNo)
        {
            var fileManager = GetLocalFileManager();

            var validFileSetup = fileManager.DirectoryExists("SetupOperations");
            if (validFileSetup.InternalException == null)
            {
                if (validFileSetup.Result == false)
                {
                    var createDirectory = fileManager.CreateDirectory("SetupOperations");
                    if (createDirectory.InternalException != null)
                    {
                        LoggerError.Fatal($"An Error Occurred FileOperations=> createDirectory : Error Message : {createDirectory.InternalException.Message}");
                        return false;
                    }
                }

                var enterSetupOperations = fileManager.EnterDirectoryPath("SetupOperations");

                if (enterSetupOperations.InternalException == null)
                {
                    var validTaskNo = fileManager.DirectoryExists(taskNo.ToString());

                    if (validTaskNo.InternalException != null)
                    {
                        LoggerError.Fatal($"An Error Occurred validTaskNo => Error Message : {validTaskNo.InternalException.Message}");
                        return false;
                    }
                    else
                    {
                        if (validTaskNo.Result == false)
                        {
                            var createDirectoryTask = fileManager.CreateDirectory(taskNo.ToString());
                            if (createDirectoryTask.InternalException != null)
                            {
                                LoggerError.Fatal($"An Error Occurred createDirectoryTask => Error Message : {createDirectoryTask.InternalException.Message}");
                                return false;
                            }
                        }

                        var enterTask = fileManager.EnterDirectoryPath(taskNo.ToString());

                        if (enterTask.InternalException == null)
                        {
                            var saveFile = fileManager.SaveFile(fileName, stream, true);
                            if (saveFile.InternalException == null)
                            {
                                return true;
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred FileOperations=> fileManager.SaveFile() : Error Message : {saveFile.InternalException.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred FileOperations=> Enter task: Error Message : {enterTask.InternalException.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred FileOperations=> fileManager.EnterDirectoryPath() : Error Message : {enterSetupOperations.InternalException.Message}");
                    return false;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred FileOperations=> validFileSetup : Error Message : {validFileSetup.InternalException.Message}");
                return false;
            }
        }


        private IFileManager GetLocalFileManager()
        {
            var rootFolder = Environment.GetEnvironmentVariable("partner_client_root");

            IFileManager fileManager = new RezaB.Files.Local.LocalFileManager(rootFolder);

            return fileManager;
        }
    }
}