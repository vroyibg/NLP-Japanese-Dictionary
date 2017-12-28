﻿using NLPJapaneseDictionary.Helpers;
using NLPJDict.DatabaseTable.NLPJDictCore;
using NLPJDict.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPJDict.NLPJDictCore.DatabaseTable
{
    public enum OcrEngineType
    {
        NlpJdict = 0,
        Microsoft = 1
    }

    public class GeneralPreference 
    {
        public enum PreferenceChanged
        {
            ReadMode,
            OmitCtrl,
            TTSVoice,
            TTSSpeed,
            ShowReading,
            ShowPronunication,
            OcrEngine
        }

        public const string USER_PREF = "Prefs.db";
        public static readonly string USER_PREF_FILE_PATH = Locations.APP_USER_FOLDER + Path.DirectorySeparatorChar + USER_PREF;
        private static Database userPrefDatabase;

        public delegate void DatabaseChangedHandler(PreferenceChanged preferenceChanged);
        public event DatabaseChangedHandler DatabaseChangedEvent;

        private bool isModified { get; set; }
        
        private int uniqueId;
        [SQLite.PrimaryKey]
        public int UniqueId
        {
            get { return uniqueId; }
            set
            {
                if (uniqueId == value)
                    return;

                uniqueId = value;
                isModified = true;
            }
        }

        private bool isReadNightMode;
        public bool IsReadNightMode
        {
            get { return isReadNightMode; }
            set
            {
                if (isReadNightMode == value)
                    return;

                isReadNightMode = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.ReadMode);
            }
        }

        private bool isOmitCtrl;
        public bool IsOmitCtrl
        {
            get { return isOmitCtrl; }
            set
            {
                if (isOmitCtrl == value)
                    return;

                isOmitCtrl = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.OmitCtrl);
            }
        }

        private string ttsVoice = null;
        public string TtsVoice
        {
            get { return ttsVoice; }
            set
            {
                if (ttsVoice == value)
                    return;

                ttsVoice = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.TTSVoice);
            }
        }

        private int ttsSpeed = 0;
        public int TtsSpeed
        {
            get { return ttsSpeed; }
            set
            {
                if (ttsSpeed == value)
                    return;

                ttsSpeed = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.TTSSpeed);
            }
        }

        private bool isShowReading;
        public bool IsShowReading
        {
            get { return isShowReading; }
            set
            {
                if (isShowReading == value)
                    return;

                isShowReading = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.ShowReading);
            }
        }

        private bool isShowPronun;
        public bool IsShowPronun
        {
            get { return isShowPronun; }
            set
            {
                if (isShowPronun == value)
                    return;

                isShowPronun = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.ShowPronunication);
            }
        }

        private OcrEngineType ocrEngine;
        public OcrEngineType OcrEngine
        {
            get { return ocrEngine; }
            set
            {
                if (ocrEngine == value)
                    return;

                ocrEngine = value;
                isModified = true;
                DatabaseChangedEvent?.Invoke(PreferenceChanged.OcrEngine);
            }
        }

        private bool isShownFirtNoitfy;
        public bool IsShownFirtNoitfy
        {
            get { return isShownFirtNoitfy; }
            set
            {
                if (isShownFirtNoitfy == value)
                    return;

                isShownFirtNoitfy = value;
                isModified = true;
            }
        }

        private double windowTop;
        public double WindowTop
        {
            get { return windowTop; }
            set
            {
                if (windowTop == value)
                    return;

                windowTop = value;
                isModified = true;
            }
        }

        private double windowLeft;
        public double WindowLeft
        {
            get { return windowLeft; }
            set
            {
                if (windowLeft == value)
                    return;

                windowLeft = value;
                isModified = true;
            }
        }

        private double windowWidth = System.Windows.SystemParameters.VirtualScreenWidth / 3;
        public double WindowWidth
        {
            get { return windowWidth; }
            set
            {
                if (windowWidth == value)
                    return;

                windowWidth = value;
                isModified = true;
            }
        }

        private double windowHeight = System.Windows.SystemParameters.VirtualScreenWidth * 2 / 3;
        public double WindowHeight
        {
            get { return windowHeight; }
            set
            {
                if (windowHeight == value)
                    return;

                windowHeight = value;
                isModified = true;
            }
        }

        private int windowState;
        public System.Windows.WindowState WindowState
        {
            get { return (System.Windows.WindowState)windowState; }
            set
            {
                var newState = (int)value;
                if (windowState == newState)
                    return;

                windowState = newState;
                isModified = true;
            }
        }

        static GeneralPreference()
        {
            if (File.Exists(USER_PREF_FILE_PATH))
            {
                try
                {
                   userPrefDatabase = new Database(USER_PREF_FILE_PATH);
                }
                catch
                {
                   DeleteAndCreateNewDatabase();
                }
            }
            else
            {
                CreateDefaultPreference();
            }
        }

        public static GeneralPreference RetrieveUserPreference()
        {
            try
            {
                return userPrefDatabase.GetTable<GeneralPreference>().First();
            }
            catch
            {
                return DeleteAndCreateNewDatabase();
            }
        }

        public void UpdateUserPreference()
        {
            try
            {
                if (isModified)
                {
                    userPrefDatabase.Update(this);
                }
            }
            catch
            {
                userPrefDatabase.DropTable<GeneralPreference>();
                userPrefDatabase.CreateTable<GeneralPreference>();
                userPrefDatabase.Insert(this);
            }
        }

        private static GeneralPreference DeleteAndCreateNewDatabase()
        {
            if (userPrefDatabase != null)
                userPrefDatabase.Close();
            File.Delete(USER_PREF_FILE_PATH);
            return CreateDefaultPreference();
        }

        private static GeneralPreference CreateDefaultPreference()
        {
            if (!Directory.Exists(Locations.APP_USER_FOLDER))
                Directory.CreateDirectory(Locations.APP_USER_FOLDER);

            userPrefDatabase = new Database(USER_PREF_FILE_PATH);
            var UserPrefs = GetDefaultPreference();
            userPrefDatabase.CreateTable<GeneralPreference>();
            userPrefDatabase.Insert(UserPrefs);
            return UserPrefs;
        }

        private static GeneralPreference GetDefaultPreference()
        {
            GeneralPreference userPrefs = new GeneralPreference();
            userPrefs.IsReadNightMode = false;
            userPrefs.IsShowPronun = true;
            userPrefs.IsShowReading = true;
            userPrefs.OcrEngine = OcrEngineType.NlpJdict;
            userPrefs.IsShownFirtNoitfy = false;
            userPrefs.TtsSpeed = 0;
            userPrefs.isOmitCtrl = false;

            userPrefs.WindowState = System.Windows.WindowState.Normal;
            userPrefs.WindowWidth = System.Windows.SystemParameters.VirtualScreenWidth/3;
            userPrefs.WindowHeight = System.Windows.SystemParameters.VirtualScreenHeight*2/3;
            userPrefs.WindowTop = 0;
            userPrefs.WindowLeft = 0;

            return userPrefs;
        }

        public static void Close()
        {
            if (userPrefDatabase != null)
                userPrefDatabase.Close();
        }
    }
}