using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    public class TicketDetailPresenter : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _ticketListViewAsset;
        [SerializeField] private VisualTreeAsset _chatMessageTemplate;

        private VisualTreeAsset _detailViewAsset; // Placeholder since it's instantiated via lists
        private VisualElement _root;
        private string _currentTicketId;

        // UI elements
        private Label _lblTitle;
        private Label _lblStatus;
        private Label _lblCreatedAt;
        private Label _lblDescription;
        private Label _lblDiffContent;
        private Label _lblAiSummary;

        private ScrollView _scvChatMessages;
        private TextField _txtMessage;
        private Button _btnSendMsg;
        private Button _btnBack;

        public void Initialize(VisualElement root, string ticketId)
        {
            _root = root;
            _currentTicketId = ticketId;

            _lblTitle = _root.Q<Label>("LblTicketTitle");
            _lblStatus = _root.Q<Label>("LblStatus");
            _lblCreatedAt = _root.Q<Label>("LblCreatedAt");
            _lblDescription = _root.Q<Label>("LblDescription");
            _lblDiffContent = _root.Q<Label>("LblDiffContent");
            _lblAiSummary = _root.Q<Label>("LblAiSummary");

            _scvChatMessages = _root.Q<ScrollView>("ScvChatMessages");
            _txtMessage = _root.Q<TextField>("TxtMessage");
            _btnSendMsg = _root.Q<Button>("BtnSendMsg");

            // Assuming Header has a home/back button. We use Home.
            var btnHome = _root.Q<Button>("BtnHome");
            if (btnHome != null) btnHome.clicked += GoBackToList;

            if (_btnSendMsg != null)
            {
                _btnSendMsg.clicked -= OnSendChatMessage;
                _btnSendMsg.clicked += OnSendChatMessage;
            }

            LoadTicketDetails();
        }

        private void OnDisable()
        {
            if (_btnSendMsg != null) _btnSendMsg.clicked -= OnSendChatMessage;
        }

        private void LoadTicketDetails()
        {
            // Fetch mockup ticket based on the list logic API
            // For MVP, we will just set mock text directly
            _lblTitle.text = $"Ticket Detail ({_currentTicketId})";
            _lblStatus.text = "[Open]";
            _lblDescription.text = "This is a requested feature to add a new network layer.";
            _lblDiffContent.text = "--- a/old_file.cs\n+++ b/new_file.cs\n@@ -1,3 +1,4 @@\n+ using UnityEngine.Networking;\n- using System.Net;";
            _lblAiSummary.text = "The code looks okay, but ensure that UnityWebRequest is properly disposed in a `using` block to avoid memory leaks.";

            // Add an initial mock AI message
            AddChatMessage("AI", "Hello! I am the review AI. Do you have any questions about my review?", false);
        }

        private void OnSendChatMessage()
        {
            string msg = _txtMessage.value;
            if (string.IsNullOrEmpty(msg)) return;

            // Add our own message to the view
            AddChatMessage("User", msg, true);
            _txtMessage.value = "";

            // Simulate AI reply (Here you would call ApiClient)
            Invoke(nameof(SimulateAiReply), 1.0f);
        }

        private void SimulateAiReply()
        {
            AddChatMessage("AI", "I understand. Let me verify that part of the code.", false);
        }

        private void AddChatMessage(string sender, string content, bool isSelf)
        {
            if (_chatMessageTemplate == null || _scvChatMessages == null) return;

            var msgEl = _chatMessageTemplate.Instantiate();
            var container = msgEl.Q<VisualElement>("MsgContainer");
            var bubble = msgEl.Q<VisualElement>("Bubble");
            
            msgEl.Q<Label>("LblSender").text = sender;
            msgEl.Q<Label>("LblContent").text = content;

            if (isSelf)
            {
                container.style.alignSelf = Align.FlexEnd;
                bubble.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#1E5128", out var c) ? c : Color.green);
            }
            else
            {
                container.style.alignSelf = Align.FlexStart;
                bubble.style.backgroundColor = new StyleColor(ColorUtility.TryParseHtmlString("#252526", out var c2) ? c2 : Color.grey);
            }

            _scvChatMessages.Add(msgEl);
            
            // Scroll to bottom manually (simple hack for UIElements)
            _scvChatMessages.schedule.Execute(() => _scvChatMessages.scrollOffset = new Vector2(0, float.MaxValue)).StartingIn(50);
        }

        private void GoBackToList()
        {
            var presenter = FindObjectOfType<TicketListPresenter>();
            if (presenter != null)
            {
                presenter.GoToList();
            }
        }
    }
}