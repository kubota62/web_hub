using System;
using System.Collections.Generic;

namespace AIRegistry.Models
{
    [Serializable]
    public class User
    {
        public string Id;
        public string Username;
    }

    [Serializable]
    public class ReviewTicket
    {
        public string Id;
        public string Title;
        public string Description;
        public string AuthorId;
        public string DiffContent;
        public List<string> Tags;
        public string CreatedAt;
        public string Status; // e.g., "Open", "Reviewed", "Closed"
    }

    [Serializable]
    public class AiReviewResult
    {
        public string TicketId;
        public string Summary;
        public List<AiReviewComment> Comments;
    }

    [Serializable]
    public class AiReviewComment
    {
        public string FilePath;
        public int LineNumber;
        public string Comment;
    }

    [Serializable]
    public class ChatMessage
    {
        public string Id;
        public string TicketId;
        public string Sender; // "User" or "AI"
        public string Content;
        public string Timestamp;
    }
}