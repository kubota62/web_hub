using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    public class TicketListPresenter : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _ticketItemTemplate;
        [SerializeField] private VisualTreeAsset _createTicketViewAsset;
        [SerializeField] private VisualTreeAsset _ticketDetailViewAsset;
        [SerializeField] private VisualTreeAsset _ticketListViewAsset;

        private VisualElement _root;
        private ScrollView _scvTicketList;
        private Button _btnCreateTicket;
        private Button _btnLogout;

        public void Initialize(VisualElement contentRoot)
        {
            _root = contentRoot;

            _scvTicketList = _root.Q<ScrollView>("ScvTicketList");
            _btnCreateTicket = _root.Q<Button>("BtnCreateTicket");
            
            // Header buttons and duplicates are now handled by AppFlowManager
            // Note: We leave BtnCreateTicket handled here because it's specific to this view
            if (_btnCreateTicket != null)
            {
                _btnCreateTicket.clicked -= GoToCreateTicket;
                _btnCreateTicket.clicked += GoToCreateTicket;
            }

            LoadTickets();
        }

        public void GoToList()
        {
            AppFlowManager.Instance.ShowView(_ticketListViewAsset);
            Initialize(AppFlowManager.Instance.GetComponent<UIDocument>().rootVisualElement.Q("MainContent"));
        }

        private void OnDisable()
        {
            if (_btnCreateTicket != null) _btnCreateTicket.clicked -= GoToCreateTicket;
        }

        private void LoadTickets()
        {
            if (_scvTicketList == null) return;
            _scvTicketList.Clear();

            ApiClient.Instance.GetTickets((success, tickets) =>
            {
                if (success && tickets != null)
                {
                    foreach (var t in tickets)
                    {
                        var ticketEl = _ticketItemTemplate.Instantiate();
                        ticketEl.Q<Label>("LblTitle").text = t.Title;
                        ticketEl.Q<Label>("LblStatus").text = $"[{t.Status}]";
                        ticketEl.Q<Label>("LblCreatedAt").text = t.CreatedAt;

                        var btnView = ticketEl.Q<Button>("BtnView");
                        btnView.clicked += () => GoToTicketDetail(t.Id);

                        _scvTicketList.Add(ticketEl);
                        
                        // Add spacing
                        var spacer = new VisualElement();
                        spacer.style.height = 8;
                        _scvTicketList.Add(spacer);
                    }
                }
            });
        }

        private void GoToCreateTicket()
        {
            var presenter = FindObjectOfType<TicketCreatePresenter>();
            if (presenter != null)
            {
                presenter.GoToCreate();
            }
        }

        private void GoToTicketDetail(string ticketId)
        {
            AppFlowManager.Instance.ShowView(_ticketDetailViewAsset);
            var presenter = FindObjectOfType<TicketDetailPresenter>();
            if (presenter != null)
            {
                presenter.Initialize(AppFlowManager.Instance.GetComponent<UIDocument>().rootVisualElement.Q("MainContent"), ticketId);
            }
        }

    }
}