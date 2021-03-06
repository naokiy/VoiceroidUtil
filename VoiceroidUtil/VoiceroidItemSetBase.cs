﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using RucheHome.Util;
using RucheHome.Voiceroid;

namespace VoiceroidUtil
{
    /// <summary>
    /// VOICEROID識別IDに紐付くアイテムセットを保持するジェネリッククラス。
    /// </summary>
    /// <typeparam name="TItem">
    /// VOICEROID識別IDに紐付くアイテムの型。
    /// IVoiceroidItem インタフェースを実装し、
    /// VOICEROID識別IDを受け取るコンストラクタを持つ必要がある。
    /// </typeparam>
    /// <remarks>
    /// 内容が変更されると、インデクサを対象として PropertyChanged イベントが発生する。
    /// </remarks>
    [DataContract(Namespace = "")]
    public abstract class VoiceroidItemSetBase<TItem> : BindableBase, IEnumerable<TItem>
        where TItem : IVoiceroidItem
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public VoiceroidItemSetBase()
        {
            // イベントハンドラ追加のためにプロパティ経由で設定
            this.Table = new InnerList();
        }

        /// <summary>
        /// VOICEROID識別IDに対応するアイテムを取得するインデクサ。
        /// </summary>
        /// <param name="id">VOICEROID識別ID。</param>
        public TItem this[VoiceroidId id] => this.GetItem(id);

        /// <summary>
        /// 列挙子を取得する。
        /// </summary>
        /// <returns>列挙子。</returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var id in this.VoiceroidIds)
            {
                yield return this.GetItem(id);
            }
        }

        /// <summary>
        /// 全VOICEROID識別IDの列挙を取得する。
        /// </summary>
        protected static IEnumerable<VoiceroidId> AllVoiceroidIds { get; } =
            (VoiceroidId[])Enum.GetValues(typeof(VoiceroidId));

        /// <summary>
        /// アイテムセットとして保持するVOICEROID識別ID列挙を取得する。
        /// </summary>
        /// <remarks>
        /// 既定では AllVoiceroidIds を返す。
        /// 特定の識別IDを除外する場合は派生クラスでオーバライドすること。
        /// その際、 DataContract のデシリアライズに対応するため、
        /// 非 static なフィールドを生成しないように注意すること。
        /// </remarks>
        protected virtual IEnumerable<VoiceroidId> VoiceroidIds => AllVoiceroidIds;

        /// <summary>
        /// VOICEROID識別IDに対応する TItem インスタンスを取得する。
        /// </summary>
        /// <param name="id">VOICEROID識別ID。</param>
        /// <returns>TItem インスタンス。</returns>
        protected TItem GetItem(VoiceroidId id)
        {
            var item = default(TItem);

            int index = this.Table.IndexOf(id);

            if (index < 0)
            {
                // 有効なIDか？
                if (!this.VoiceroidIds.Contains(id))
                {
                    throw new InvalidEnumArgumentException(
                        nameof(id),
                        (int)id,
                        id.GetType());
                }

                // アイテムを作成して追加
                item = (TItem)Activator.CreateInstance(typeof(TItem), id);
                this.Table.Add(item);
            }
            else
            {
                // 取得
                item = this.Table[index];
            }

            return item;
        }

        /// <summary>
        /// VOICEROID識別IDに対応する TItem インスタンスを設定する。
        /// </summary>
        /// <param name="id">VOICEROID識別ID。</param>
        /// <param name="item">TItem インスタンス。</param>
        protected void SetItem(VoiceroidId id, TItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.VoiceroidId != id)
            {
                throw new ArgumentException(
                    nameof(id) + @" != " + nameof(item) + @"." + nameof(VoiceroidId));
            }

            int index = this.Table.IndexOf(id);

            if (index < 0)
            {
                // 有効なIDか？
                if (!this.VoiceroidIds.Contains(id))
                {
                    throw new InvalidEnumArgumentException(
                        nameof(item) + @"." + nameof(VoiceroidId),
                        (int)id,
                        id.GetType());
                }

                // 新規追加
                this.Table.Add(item);
            }
            else
            {
                // 更新
                this.Table[index] = item;
            }
        }

        /// <summary>
        /// 内部リストクラス。
        /// </summary>
        private class InnerList : BindableCollection<TItem>
        {
            /// <summary>
            /// 指定したVOICEROID識別IDを持つ要素が存在するか否かを取得する。
            /// </summary>
            /// <param name="id">VOICEROID識別ID。</param>
            /// <returns></returns>
            public bool Contains(VoiceroidId id) =>
                this.Any(item => item.VoiceroidId == id);

            /// <summary>
            /// 指定したVOICEROID識別IDを持つ要素のインデックスを取得する。
            /// </summary>
            /// <param name="id">VOICEROID識別ID。</param>
            /// <returns>インデックス。見つからなければ -1 。</returns>
            public int IndexOf(VoiceroidId id)
            {
                for (int i = 0; i < this.Count; ++i)
                {
                    if (this[i].VoiceroidId == id)
                    {
                        return i;
                    }
                }
                return -1;
            }

            #region Collection<TItem> のオーバライド

            protected override void InsertItem(int index, TItem item)
            {
                if (item == null)
                {
                    return;
                }

                // ID重複なら無視
                if (this.Contains(item.VoiceroidId))
                {
                    return;
                }

                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, TItem item)
            {
                if (item == null)
                {
                    return;
                }

                // ID重複なら無視
                var id = item.VoiceroidId;
                if (this[index].VoiceroidId != id && this.Contains(id))
                {
                    return;
                }

                base.SetItem(index, item);
            }

            #endregion
        }

        /// <summary>
        /// 内部リストを取得または設定する。
        /// </summary>
        [DataMember]
        private InnerList Table
        {
            get => this.table;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value != this.table)
                {
                    // コンストラクタ呼び出しか？
                    bool construct = (this.table == null);

                    if (!construct)
                    {
                        this.table.CollectionChanged -= this.OnTableCollectionChanged;
                        this.table.ItemPropertyChanged -= this.OnTableItemPropertyChanged;
                    }

                    this.table = value;
                    this.table.CollectionChanged += this.OnTableCollectionChanged;
                    this.table.ItemPropertyChanged += this.OnTableItemPropertyChanged;

                    // コンストラクタ呼び出しでなければインデクサ変更通知
                    if (!construct)
                    {
                        this.RaiseIndexerPropertyChanged();
                    }
                }
            }
        }
        private InnerList table = null;

        /// <summary>
        /// インデクサを対象として PropertyChanged イベントを発生させる。
        /// </summary>
        private void RaiseIndexerPropertyChanged() =>
            this.RaisePropertyChanged(Binding.IndexerName);

        /// <summary>
        /// 内部リストのコレクション内容変更時に呼び出される。
        /// </summary>
        private void OnTableCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
            =>
            this.RaiseIndexerPropertyChanged(); // インデクサ変更通知

        /// <summary>
        /// 内部リストのアイテム内容変更時に呼び出される。
        /// </summary>
        private void OnTableItemPropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
            =>
            this.RaiseIndexerPropertyChanged(); // インデクサ変更通知

        /// <summary>
        /// デシリアライズの直前に呼び出される。
        /// </summary>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            // null 回避
            this.Table = new InnerList();
        }

        #region IEnumerable の明示的実装

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion
    }
}
