using System;
using System.Collections.Generic;

namespace AIRegistry.Models
{
    /// <summary>
    /// ユーザー情報を管理するクラスです。
    /// ログインユーザーの識別や表示名の保持に使用されます。
    /// </summary>
    [Serializable]
    public class User
    {
        /// <summary> ユーザーを一意に識別するID（UUID等） </summary>
        public string Id;
        /// <summary> アプリケーション内で表示されるユーザーの名前 </summary>
        public string Username;
    }

    /// <summary>
    /// コードレビューなどの依頼（リクエスト）を管理するチケットクラスです。
    /// 一覧表示や詳細表示、AIレビューの対象となります。
    /// </summary>
    [Serializable]
    public class ReviewTicket
    {
        /// <summary> チケットを一意に識別するID </summary>
        public string Id;
        /// <summary> チケットのメインタイトル </summary>
        public string Title;
        /// <summary> 依頼内容や背景を記述した詳細説明 </summary>
        public string Description;
        /// <summary> このチケットを作成したユーザーのID </summary>
        public string AuthorId;
        /// <summary> レビュー対象となるコードの差分やテキスト内容 </summary>
        public string DiffContent;
        /// <summary> 検索や分類に使用するタグ文字列のリスト </summary>
        public List<string> Tags;
        /// <summary> 作成された日時を表す文字列 </summary>
        public string CreatedAt;
        /// <summary> チケットの現在の状態（"Open", "Reviewed", "Closed" など） </summary>
        public string Status;
    }

    /// <summary>
    /// AIがチケットを分析した結果をまとめたクラスです。
    /// </summary>
    [Serializable]
    public class AiReviewResult
    {
        /// <summary> レビュー対象のチケットID </summary>
        public string TicketId;
        /// <summary> AIによる全体的な分析の要約 </summary>
        public string Summary;
        /// <summary> 指摘された具体的な修正案（コメント）のリスト </summary>
        public List<AiReviewComment> Comments;
    }

    /// <summary>
    /// AIが特定のコード箇所に対して行った指摘事項の詳細です。
    /// </summary>
    [Serializable]
    public class AiReviewComment
    {
        /// <summary> 指摘対象のファイルパス </summary>
        public string FilePath;
        /// <summary> 指摘箇所の開始行番号 </summary>
        public int LineNumber;
        /// <summary> AIからの具体的なアドバイスや指摘内容 </summary>
        public string Comment;
    }

    /// <summary>
    /// チケットに紐付くチャットメッセージの単位です。
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        /// <summary> メッセージ自身を一意に識別するID </summary>
        public string Id;
        /// <summary> 関連するチケットのID </summary>
        public string TicketId;
        /// <summary> 発言者の種類（例: "User", "AI"） </summary>
        public string Sender;
        /// <summary> メッセージの本文テキスト </summary>
        public string Content;
        /// <summary> メッセージが送信された日時の文字列 </summary>
        public string Timestamp;
    }
}