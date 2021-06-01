using NLog;
using RezaB.Files;
using RezaB.Data.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MasterISS_Partner_WebSite.ViewModels;

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
                        LoggerError.Fatal($"An Error Occurred SaveSetupFile=> createDirectory : Error Message : {createDirectory.InternalException.Message}");
                        return false;
                    }
                }

                var enterSetupOperations = fileManager.EnterDirectoryPath("SetupOperations");

                if (enterSetupOperations.InternalException == null)
                {
                    var validTaskNo = fileManager.DirectoryExists(taskNo.ToString());

                    if (validTaskNo.InternalException != null)
                    {
                        LoggerError.Fatal($"An Error Occurred SaveSetupFile validTaskNo => Error Message : {validTaskNo.InternalException.Message}");
                        return false;
                    }
                    else
                    {
                        if (validTaskNo.Result == false)
                        {
                            var createDirectoryTask = fileManager.CreateDirectory(taskNo.ToString());
                            if (createDirectoryTask.InternalException != null)
                            {
                                LoggerError.Fatal($"An Error Occurred  SaveSetupFile createDirectoryTask => Error Message : {createDirectoryTask.InternalException.Message}");
                                return false;
                            }
                        }

                        var enterTask = fileManager.EnterDirectoryPath(taskNo.ToString());

                        if (enterTask.InternalException == null)
                        {
                            var saveFile = fileManager.SaveFile(fileName, stream, false);
                            if (saveFile.InternalException == null)
                            {
                                return true;
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred SaveSetupFile=> fileManager.SaveFile() : Error Message : {saveFile.InternalException.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred SaveSetupFile=> Enter task: Error Message : {enterTask.InternalException.Message}");
                            return false;
                        }
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred SaveSetupFile=> fileManager.EnterDirectoryPath() : Error Message : {enterSetupOperations.InternalException.Message}");
                    return false;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred SaveSetupFile=> validFileSetup : Error Message : {validFileSetup.InternalException.Message}");
                return false;
            }
        }

        public IEnumerable<string> GetSetupFileList(long taskNo)
        {
            var fileManager = GetLocalFileManager();

            var validFileSetup = fileManager.DirectoryExists("SetupOperations");
            if (validFileSetup.InternalException == null)
            {
                if (validFileSetup.Result == false)
                {
                    return null;
                }

                var enterSetupOperations = fileManager.EnterDirectoryPath("SetupOperations");
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
                            var getList = fileManager.GetFileList();

                            if (getList.InternalException == null)
                            {
                                return getList.Result;
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred GetSetupFile=> getList : Error Message : {getList.InternalException.Message}");
                                return null;
                            }
                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred GetSetupFile=> enterTaskFile : Error Message : {enterTaskFile.InternalException.Message}");
                            return null;
                        }
                    }
                    else
                    {
                        LoggerError.Fatal($"An Error Occurred GetSetupFile=> validTaskFile : Error Message : {validTaskFile.InternalException.Message}");
                        return null;
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred GetSetupFile=> enterSetupOperations : Error Message : {enterSetupOperations.InternalException.Message}");
                    return null;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred GetSetupFile=> validFileSetup : Error Message : {validFileSetup.InternalException.Message}");
                return null;
            }
        }

        public Stream GetFile(long taskNo, string filename)
        {
            var fileManager = GetLocalFileManager();

            var validFileSetup = fileManager.DirectoryExists("SetupOperations");
            if (validFileSetup.InternalException == null)
            {
                if (validFileSetup.Result == false)
                {
                    return null;
                }

                var enterSetupOperations = fileManager.EnterDirectoryPath("SetupOperations");
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
                                    return getFile.Result;
                                }
                                else
                                {
                                    LoggerError.Fatal($"An Error Occurred GetSetupFile=> getFile : Error Message : {getFile.InternalException.Message}");
                                    return null;
                                }
                            }
                            else
                            {
                                LoggerError.Fatal($"An Error Occurred GetSetupFile=> validFile : Error Message : {validFile.InternalException.Message}");
                                return null;
                            }

                        }
                        else
                        {
                            LoggerError.Fatal($"An Error Occurred GetSetupFile=> enterTaskFile : Error Message : {enterTaskFile.InternalException.Message}");
                            return null;
                        }
                    }
                    else
                    {
                        LoggerError.Fatal($"An Error Occurred GetSetupFile=> validTaskFile : Error Message : {validTaskFile.InternalException.Message}");
                        return null;
                    }
                }
                else
                {
                    LoggerError.Fatal($"An Error Occurred GetSetupFile=> enterSetupOperations : Error Message : {enterSetupOperations.InternalException.Message}");
                    return null;
                }
            }
            else
            {
                LoggerError.Fatal($"An Error Occurred GetSetupFile=> validFileSetup : Error Message : {validFileSetup.InternalException.Message}");
                return null;
            }
        }

        public KeyValuePair<bool, string> ValidFiles(IEnumerable<HttpPostedFileBase> files, bool couldBePdf)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileSize = Convert.ToDecimal(file.ContentLength) / 1024 / 1024;
                    if (Properties.Settings.Default.FileSizeLimit > fileSize)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        var acceptedExtensionList = new List<string>();

                        if (couldBePdf)
                        {
                            string[] acceptedExtensions = { ".jpg", ".pdf", ".png", ".jpeg" };

                            acceptedExtensionList.AddRange(acceptedExtensions);
                        }
                        else
                        {
                            string[] acceptedExtensions = { ".jpg", ".png", ".jpeg" };

                            acceptedExtensionList.AddRange(acceptedExtensions);
                        }

                        if (acceptedExtensionList.Contains(extension))
                        {
                            return new KeyValuePair<bool, string>(true, null);
                        }

                        return new KeyValuePair<bool, string>(false, Localization.View.FaultyFormat);
                    }
                    return new KeyValuePair<bool, string>(false, Localization.View.MaxFileSizeError);
                }
            }
            return new KeyValuePair<bool, string>(false, Localization.View.SelectFile);
        }

        private IFileManager GetLocalFileManager()
        {
            var rootFolder = Environment.GetEnvironmentVariable(Properties.Settings.Default.EnvironmentVariableRoot);

            IFileManager fileManager = new RezaB.Files.Local.LocalFileManager(Properties.Settings.Default.EnvironmentVariableRoot); //"partner_client_root"rootFolder

            return fileManager;
        }
    }
}