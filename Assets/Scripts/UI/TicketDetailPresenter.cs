using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    /// <summary>
    /// チケット詳細表示とチャット機能を管理するプレゼンタークラスです。
    /// </summary>
    public class TicketDetailPresenter : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private VisualTreeAsset _chatMessageTemplate;

        private VisualElement _root;
        private string _currentTicketId;

        // UI要素
        private Label _lblTitle;
        private Label _lblStatus;
        private Label _lblCreatedAt;
        private Label _lblDescription;
        private Label _lblDiffContent;
        private Label _lblAiSummary;

        private ScrollView _scvChatMessages;
        private TextField _txtMessage;
        private Button _btnSendMsg;

        /// <summary>
        /// 詳細画面を初期化します。
        /// </summary>
        /// <param name="root">詳細画面のルート要素</param>
        /// <param name="ticketId">対象チケットID</param>
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

            // ヘッダー内の共通ボタンに対する「戻る」処理
            var btnHome = _root.Q<Button>("BtnHome");
            if (btnHome != null)
            {
                btnHome.clicked -= GoBackToList;
                btnHome.clicked += GoBackToList;
            }

            if (_btnSendMsg != null)
            {
                _btnSendMsg.clicked -= OnSendChatMessage;
                _btnSendMsg.clicked += OnSendChatMessage;
            }

            LoadTicketDetails();
        }

        /// <summary>
        /// オブジェクトが無効になった際のクリーンアップ処理を行います。
        /// </summary>
        private void OnDisable()
        {
            if (_btnSendMsg != null) _btnSendMsg.clicked -= OnSendChatMessage;
        }

        /// <summary>
        /// チケットの詳細情報を読み込み、UIに反映します。
        /// </summary>
        private void LoadTicketDetails()
        {
            // UIのリセット
            _lblTitle.text = "Loading...";
            _scvChatMessages.Clear();

            // チケット基本情報の取得
            ApiClient.Instance.GetTicketDetail(_currentTicketId, (success, ticket) =>
            {
                if (success && ticket != null)
                {
                    _lblTitle.text = ticket.Title;
                    _lblStatus.text = $"[{ticket.Status}]";
                    _lblCreatedAt.text = ticket.CreatedAt;
                    _lblDescription.text = ticket.Description;
                    _lblDiffContent.text = ticket.DiffContent;
                }
            });

            // AIレビュー結果の取得
            ApiClient.Instance.GetAiReviewResult(_currentTicketId, (success, result) =>
            {
                if (success && result != null)
                {
                    _lblAiSummary.text = result.Summary;
                }
            });

            // チャットの初期化メッセージ
            AddChatMessage("AI", "こんにちは！レビューAIです。何か質問はありますか？", false);
        }

        /// <summary>
        /// チャットメッセージの送信ボタンが押された際の処理です。
        /// </summary>
        private void OnSendChatMessage()
        {
            string msg = _txtMessage.value;
            if (string.IsNullOrEmpty(msg)) return;

            AddChatMessage("自分", msg, true);
            _txtMessage.value = "";

            Invoke(nameof(SimulateAiReply), 1.0f);
        }

        /// <summary>
        /// AIの返答をシミュレートするためのメソッドです。
        /// </summary>
        private void SimulateAiReply()
        {
            AddChatMessage("AI", "ご質問ありがとうございます。そのコード箇所について調査してみますね。", false);
        }

        /// <summary>
        /// チャット画面にメッセージを追加します。
        /// </summary>
        /// <param name="sender">送信者名</param>
        /// <param name="content">メッセージ内容</param>
        /// <param name="isSelf">自分自身の発言かどうか</param>
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
                bubble.style.backgroundColor = new StyleColor(new Color(0.12f, 0.32f, 0.16f));
            }
            else
            {
                container.style.alignSelf = Align.FlexStart;
                bubble.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
            }

            _scvChatMessages.Add(msgEl);
            
            _scvChatMessages.schedule.Execute(() => 
                _scvChatMessages.scrollOffset = new Vector2(0, float.MaxValue)
            ).StartingIn(50);
        }

        /// <summary>
        /// チケット一覧画面へ戻ります。
        /// </summary>
        private void GoBackToList()
        {
            // AppFlowManagerに遷移を依頼
            AppFlowManager.Instance.GoToTicketList();
        }
    }
}