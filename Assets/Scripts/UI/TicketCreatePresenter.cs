using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    public class TicketCreatePresenter : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _ticketListViewAsset;

        [SerializeField] private VisualTreeAsset _createTicketViewAsset;
        private VisualElement _root;
        private TextField _txtTitle;
        private TextField _txtDescription;
        private TextField _txtDiff;
        private Button _btnSubmit;
        private Button _btnCancel;

        // Tags and Modal UI
        private VisualElement _tagListContainer;
        private Button _btnAddTag;
        private VisualElement _modalAddTag;
        private TextField _txtNewTagName;
        private Label _lblTagError;
        private Button _btnConfirmAddTag;
        private Button _btnCancelTag;

        private List<string> _availableTags = new List<string> { "Bug", "Feature", "Refactor", "Network", "UI" };
        private List<Toggle> _tagToggles = new List<Toggle>();

        public void GoToCreate()
        {
            if (_createTicketViewAsset != null)
            {
                AppFlowManager.Instance.ShowView(_createTicketViewAsset);
                Initialize(AppFlowManager.Instance.GetComponent<UIDocument>().rootVisualElement.Q("MainContent"));
            }
        }

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

        private void OpenTagModal()
        {
            if (_modalAddTag != null)
            {
                _modalAddTag.style.display = DisplayStyle.Flex;
                _txtNewTagName.value = "";
                _lblTagError.style.display = DisplayStyle.None;
            }
        }

        private void CloseTagModal()
        {
            if (_modalAddTag != null)
            {
                _modalAddTag.style.display = DisplayStyle.None;
            }
        }

        private void AddNewTag()
        {
            string newTag = _txtNewTagName.value.Trim();
            if (string.IsNullOrEmpty(newTag)) return;

            if (_availableTags.Contains(newTag))
            {
                _lblTagError.text = "Tag already exists.";
                _lblTagError.style.display = DisplayStyle.Flex;
                return;
            }

            _availableTags.Add(newTag);
            RefreshTagsUI();
            
            // Auto check the newly added tag
            var lastToggle = _tagToggles[_tagToggles.Count - 1];
            lastToggle.value = true;

            CloseTagModal();
        }

        private void OnDisable()
        {
            if (_btnSubmit != null) _btnSubmit.clicked -= OnSubmitClicked;
            if (_btnCancel != null) _btnCancel.clicked -= GoBackToList;
        }

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

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(diff))
            {
                Debug.LogWarning("Title and Diff are required.");
                return;
            }

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
                    Debug.Log($"Ticket Created: {result.Id}");
                    GoBackToList();
                }
                else
                {
                    Debug.LogError("Failed to create ticket.");
                }
            });
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