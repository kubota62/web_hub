using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    /// <summary>
    /// 新規チケット作成画面のロジックを管理するプレゼンタークラスです。
    /// </summary>
    public class TicketCreatePresenter : MonoBehaviour
    {
        private VisualElement _root;
        private TextField _txtTitle;
        private TextField _txtDescription;
        private TextField _txtDiff;
        private Button _btnSubmit;
        private Button _btnCancel;

        private VisualElement _tagListContainer;
        private Button _btnAddTag;
        private VisualElement _modalAddTag;
        private TextField _txtNewTagName;
        private Label _lblTagError;
        private Button _btnConfirmAddTag;
        private Button _btnCancelTag;

        private List<string> _availableTags = new List<string> { "Bug", "Feature", "Refactor", "Network", "UI" };
        private List<Toggle> _tagToggles = new List<Toggle>();

        /// <summary>
        /// 新規チケット作成画面の初期化処理を行います。
        /// </summary>
        /// <param name="root">画面のルート要素</param>
        public void Initialize(VisualElement root)
        {
            _root = root;
            
            _txtTitle = _root.Q<TextField>("TxtTitle");
            _txtDescription = _root.Q<TextField>("TxtDescription");
            _txtDiff = _root.Q<TextField>("TxtDiff");
            _btnSubmit = _root.Q<Button>("BtnSubmit");
            _btnCancel = _root.Q<Button>("BtnCancel");

            _tagListContainer = _root.Q<VisualElement>("TagListContainer");
            _btnAddTag = _root.Q<Button>("BtnAddTag");
            _modalAddTag = _root.Q<VisualElement>("ModalAddTag");
            _txtNewTagName = _root.Q<TextField>("TxtNewTagName");
            _lblTagError = _root.Q<Label>("LblTagError");
            _btnConfirmAddTag = _root.Q<Button>("BtnConfirmAddTag");
            _btnCancelTag = _root.Q<Button>("BtnCancelTag");

            _btnSubmit.clicked -= OnSubmitClicked;
            _btnSubmit.clicked += OnSubmitClicked;
            
            _btnCancel.clicked -= GoBackToList;
            _btnCancel.clicked += GoBackToList;

            _btnAddTag.clicked -= OpenTagModal;
            _btnAddTag.clicked += OpenTagModal;

            _btnConfirmAddTag.clicked -= AddNewTag;
            _btnConfirmAddTag.clicked += AddNewTag;

            _btnCancelTag.clicked -= CloseTagModal;
            _btnCancelTag.clicked += CloseTagModal;

            RefreshTagsUI();
        }

        /// <summary>
        /// タグ一覧の表示を更新します。
        /// </summary>
        private void RefreshTagsUI()
        {
            if (_tagListContainer == null) return;
            _tagListContainer.Clear();
            _tagToggles.Clear();

            foreach (var tag in _availableTags)
            {
                var toggle = new Toggle { text = tag };
                toggle.style.marginRight = 16;
                toggle.style.marginBottom = 8;
                _tagToggles.Add(toggle);
                _tagListContainer.Add(toggle);
            }
        }

        /// <summary>
        /// タグ追加モーダルを開きます。
        /// </summary>
        private void OpenTagModal()
        {
            if (_modalAddTag != null)
            {
                _modalAddTag.style.display = DisplayStyle.Flex;
                _txtNewTagName.value = "";
                _lblTagError.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// タグ追加モーダルを閉じます。
        /// </summary>
        private void CloseTagModal()
        {
            if (_modalAddTag != null) _modalAddTag.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 新しいタグを追加します。
        /// </summary>
        private void AddNewTag()
        {
            string newTag = _txtNewTagName.value.Trim();
            if (string.IsNullOrEmpty(newTag)) return;

            if (_availableTags.Contains(newTag))
            {
                _lblTagError.text = "そのタグは既に存在します。";
                _lblTagError.style.display = DisplayStyle.Flex;
                return;
            }

            _availableTags.Add(newTag);
            RefreshTagsUI();
            
            if (_tagToggles.Count > 0) _tagToggles[_tagToggles.Count - 1].value = true;
            CloseTagModal();
        }

        /// <summary>
        /// オブジェクトが無効になった際のクリーンアップ処理を行います。
        /// </summary>
        private void OnDisable()
        {
            if (_btnSubmit != null) _btnSubmit.clicked -= OnSubmitClicked;
            if (_btnCancel != null) _btnCancel.clicked -= GoBackToList;
        }

        /// <summary>
        /// 提出ボタンが押された際の、チケット作成実行処理です。
        /// </summary>
        private void OnSubmitClicked()
        {
            string title = _txtTitle.value;
            string description = _txtDescription.value;
            string diff = _txtDiff.value;
            
            List<string> selectedTags = new List<string>();
            foreach (var toggle in _tagToggles)
            {
                if (toggle.value) selectedTags.Add(toggle.text);
            }

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(diff)) return;

            _btnSubmit.SetEnabled(false);

            var ticket = new ReviewTicket
            {
                Title = title,
                Description = description,
                DiffContent = diff,
                Tags = selectedTags
            };

            ApiClient.Instance.CreateTicket(ticket, (success, result) =>
            {
                _btnSubmit.SetEnabled(true);
                if (success)
                {
                    // AppFlowManagerに一覧への復帰を依頼
                    AppFlowManager.Instance.GoToTicketList();
                }
            });
        }

        /// <summary>
        /// 一覧画面へ戻ります。
        /// </summary>
        private void GoBackToList()
        {
            // AppFlowManagerに一覧への復帰を依頼
            AppFlowManager.Instance.GoToTicketList();
        }
    }
}