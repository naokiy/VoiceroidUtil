﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using RucheHome.Util;

namespace VoiceroidUtil
{
    /// <summary>
    /// アプリケーション設定クラス。
    /// </summary>
    [DataContract(Namespace = "")]
    public class AppConfig : BindableConfigBase
    {
        /// <summary>
        /// 既定の保存先ディレクトリパス。
        /// </summary>
        public static readonly string DefaultSaveDirectoryPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                @"VoiceroidWaveFiles");

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public AppConfig()
        {
            // イベントハンドラ追加のためにプロパティ経由で設定
            this.VoiceroidVisibilities = new VoiceroidVisibilitySet();
            this.YmmCharaRelations = new YmmCharaRelationSet();
        }

        /// <summary>
        /// 起動時にアプリの更新をチェックするか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsUpdateCheckingOnStartup
        {
            get => this.updateCheckingOnStartup;
            set => this.SetProperty(ref this.updateCheckingOnStartup, value);
        }
        private bool updateCheckingOnStartup = true;

        /// <summary>
        /// ウィンドウを常に最前面に表示するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsTopmost
        {
            get => this.topmost;
            set => this.SetProperty(ref this.topmost, value);
        }
        private bool topmost = false;

        /// <summary>
        /// タブ文字の入力を受け付けるか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsTabAccepted
        {
            get => this.tabAccepted;
            set => this.SetProperty(ref this.tabAccepted, value);
        }
        private bool tabAccepted = true;

        /// <summary>
        /// VOICEROID表示設定セットを取得または設定する。
        /// </summary>
        [DataMember]
        public VoiceroidVisibilitySet VoiceroidVisibilities
        {
            get => this.voiceroidVisibilities;
            set =>
                this.SetPropertyWithPropertyChangedChain(
                    ref this.voiceroidVisibilities,
                    value ?? (new VoiceroidVisibilitySet()));
        }
        private VoiceroidVisibilitySet voiceroidVisibilities = null;

        /// <summary>
        /// 保存先ディレクトリパスを取得または設定する。
        /// </summary>
        [DataMember]
        public string SaveDirectoryPath
        {
            get => this.saveDirectoryPath;
            set =>
                this.SetProperty(
                    ref this.saveDirectoryPath,
                    string.IsNullOrWhiteSpace(value) ? DefaultSaveDirectoryPath : value);
        }
        private string saveDirectoryPath = DefaultSaveDirectoryPath;

        /// <summary>
        /// ファイル名フォーマットを取得または設定する。
        /// </summary>
        public FileNameFormat FileNameFormat
        {
            get => this.fileNameFormat;
            set =>
                this.SetProperty(
                    ref this.fileNameFormat,
                    Enum.IsDefined(value.GetType(), value) ?
                        value : FileNameFormat.DateTimeNameText);
        }
        private FileNameFormat fileNameFormat = FileNameFormat.DateTimeNameText;

        /// <summary>
        /// FileNameFormat プロパティのシリアライズ用ラッパプロパティ。
        /// </summary>
        [DataMember(Name = nameof(FileNameFormat))]
        private string FileNameFormatString
        {
            get => this.FileNameFormat.ToString();
            set =>
                this.FileNameFormat =
                    Enum.TryParse(value, out FileNameFormat f) ?
                        f : FileNameFormat.DateTimeNameText;
        }

        /// <summary>
        /// テキストファイルを必ず作成するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsTextFileForceMaking
        {
            get => this.textFileForceMaking;
            set => this.SetProperty(ref this.textFileForceMaking, value);
        }
        private bool textFileForceMaking = true;

        /// <summary>
        /// テキストファイルをUTF-8(BOMなし)で作成するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsTextFileUtf8
        {
            get => this.textFileUtf8;
            set => this.SetProperty(ref this.textFileUtf8, value);
        }
        private bool textFileUtf8 = false;

        /// <summary>
        /// AviUtl拡張編集ファイルを作成するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsExoFileMaking
        {
            get => this.exoFileMaking;
            set => this.SetProperty(ref this.exoFileMaking, value);
        }
        private bool exoFileMaking = false;

        /// <summary>
        /// 音声保存成功時にテキストをクリアするか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsTextClearing
        {
            get => this.textClearing;
            set => this.SetProperty(ref this.textClearing, value);
        }
        private bool textClearing = false;

        /// <summary>
        /// 音声保存時に UI Automation の利用を許可するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsUIAutomationEnabledOnSave
        {
            get => this.uiAutomationEnabledOnSave;
            set => this.SetProperty(ref this.uiAutomationEnabledOnSave, value);
        }
        private bool uiAutomationEnabledOnSave = true;

        /// <summary>
        /// 保存したファイルのパスを『ゆっくりMovieMaker』に設定するか否かを
        /// 取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsSavedFileToYmm
        {
            get => this.savedFileToYmm;
            set => this.SetProperty(ref this.savedFileToYmm, value);
        }
        private bool savedFileToYmm = true;

        /// <summary>
        /// 『ゆっくりMovieMaker』のキャラを自動選択するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsYmmCharaSelecting
        {
            get => this.ymmCharaSelecting;
            set => this.SetProperty(ref this.ymmCharaSelecting, value);
        }
        private bool ymmCharaSelecting = true;

        /// <summary>
        /// VOICEROIDと『ゆっくりMovieMaker』のキャラ名との紐付けを取得または設定する。
        /// </summary>
        [DataMember]
        public YmmCharaRelationSet YmmCharaRelations
        {
            get => this.ymmCharaRelations;
            set =>
                this.SetPropertyWithPropertyChangedChain(
                    ref this.ymmCharaRelations,
                    value ?? (new YmmCharaRelationSet()));
        }
        private YmmCharaRelationSet ymmCharaRelations = null;

        /// <summary>
        /// 『ゆっくりMovieMaker』の追加ボタンを自動押下するか否かを取得または設定する。
        /// </summary>
        [DataMember]
        public bool IsYmmAddButtonClicking
        {
            get => this.ymmAddButtonClicking;
            set => this.SetProperty(ref this.ymmAddButtonClicking, value);
        }
        private bool ymmAddButtonClicking = true;

        /// <summary>
        /// デシリアライズの直前に呼び出される。
        /// </summary>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.ResetDataMembers();
        }
    }
}
