namespace SSS.Quality1500.Presentation.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using SSS.Quality1500.Presentation.ViewModels;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Business.Models;

public class ConfigurationService : IUiConfigurationService
    {
        // Configuración de archivos
        public string VkFile { get; set; } = string.Empty;
        public string DatFile { get; set; } = string.Empty;
        public string IdxFile { get; set; } = string.Empty;
        public string ImgFileIn { get; set; } = string.Empty;
        public string ImgFileOut { get; set; } = string.Empty;
        public string PathOut { get; set; } = string.Empty;

        // Opciones de procesamiento
        public bool Is837StampingChecked { get; set; }
        public bool IsIdaChecked { get; set; }
        public bool IsZipperChecked { get; set; }
        public bool IsConduceChecked { get; set; }
        public bool IsBackupChecked { get; set; }
        public bool IsConduceDummyChecked { get; set; }
        public bool IsVersionChecked { get; set; }
        public bool IsRemoveHeaderChecked { get; set; }
        public bool Is837ToXlsChecked { get; set; }

        // Información de batches
        public ObservableCollection<string> BatchItems { get; set; } = new();
        public ObservableCollection<VkFileRecord> SelectedBatches { get; set; } = new(); 
        public DateTime SelectedDate { get; set; } = DateTime.Now;
        public string TotalImagenes { get; set; } = "0";
        public string TotalDocumentos { get; set; } = "0";

        // VdeTable

        public DataTable VdeTable { get; set; } = new();

        // Eventos para notificar cambios
        public event Action? ConfigurationChanged;

        public void UpdateConfiguration(
            string vkFile, string datFile, string idxFile, string imgFileIn, string imgFileOut, string pathOut,
            bool is837Stamping, bool isIda, bool isZipper, bool isConduce, bool isBackup,
            bool isConduceDummy, bool isVersion, bool isRemoveHeader, bool isToXls,
            DateTime selectedDate, string totalImagenes, string totalDocumentos, DataTable vdeTable)
        {
            VkFile = vkFile;
            DatFile = datFile;
            IdxFile = idxFile;
            ImgFileIn = imgFileIn;
            ImgFileOut = imgFileOut;
            Is837StampingChecked = is837Stamping;
            IsIdaChecked = isIda;
            IsZipperChecked = isZipper;
            IsConduceChecked = isConduce;
            IsBackupChecked = isBackup;
            IsConduceDummyChecked = isConduceDummy;
            IsVersionChecked = isVersion;
            IsRemoveHeaderChecked = isRemoveHeader;
            Is837ToXlsChecked = isToXls;
            SelectedDate = selectedDate;
            TotalImagenes = totalImagenes;
            TotalDocumentos = totalDocumentos;
            VdeTable = vdeTable;
            PathOut = pathOut;

            ConfigurationChanged?.Invoke();
        }

        public void ClearBatches()
        {
            SelectedBatches.Clear();
            BatchItems.Clear();
            ConfigurationChanged?.Invoke();
        }

        public void ClearAllConfiguration()
        {
            // Limpiar archivos
            VkFile = string.Empty;
            DatFile = string.Empty;
            IdxFile = string.Empty;
            ImgFileIn = string.Empty;
            ImgFileOut = string.Empty;

            // Limpiar opciones
            Is837StampingChecked = false;
            IsIdaChecked = false;
            IsZipperChecked = false;
            IsConduceChecked = true;
            IsBackupChecked = false;
            IsConduceDummyChecked = false;
            IsVersionChecked = false;
            IsRemoveHeaderChecked = false;
            Is837ToXlsChecked = false;

            // Limpiar batches
            SelectedBatches.Clear();
            BatchItems.Clear();

            // Resetear valores
            SelectedDate = DateTime.Now;
            TotalImagenes = "0";
            TotalDocumentos = "0";

            ConfigurationChanged?.Invoke();
        }

        public bool IsConfigurationComplete()
        {
            return !string.IsNullOrWhiteSpace(VkFile) &&
                   !string.IsNullOrWhiteSpace(DatFile) &&
                   !string.IsNullOrWhiteSpace(IdxFile) &&
                   !string.IsNullOrWhiteSpace(ImgFileIn) &&
                   !string.IsNullOrWhiteSpace(ImgFileOut) &&
                   BatchItems.Count > 0;
        }
    }
