using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;
using AIRegistry.Models;

namespace AIRegistry.Frontend.Controllers
{
    /// <summary>
    /// チケット一覧画面の制御を行うプレゼンタークラスです。
    /// </summary>
    public class TicketListPresenter : MonoBehaviour
    {
        [Header("Templates")]
        [SerializeField] private VisualTreeAsset _ticketItemTemplate;

        private VisualElement _root;
        private ScrollView _scvTicketList;
        private Button _btnCreateTicket;

        /// <summary>
        /// UI要素の参照取得とデータの読み込みを開始します。
        /// </summary>
        /// <param name="contentRoot">画面のルートとなる要素</param>
        public void Initialize(VisualElement contentRoot)
        {
            _root = contentRoot;

            _scvTicketList = _root.Q<ScrollView>("ScvTicketList");
            _btnCreateTicket = _root.Q<Button>("BtnCreateTicket");
            
            if (_btnCreateTicket != null)
            {
                _btnCreateTicket.clicked -= GoToCreateTicket;
                _btnCreateTicket.clicked += GoToCreateTicket;
            }

            LoadTickets();
        }

        /// <summary>
        /// オブジェクトが無効になった際のクリーンアップ処理を行います。
        /// </summary>
        private void OnDisable()
        {
            if (_btnCreateTicket != null) _btnCreateTicket.clicked -= GoToCreateTicket;
        }

        /// <summary>
        /// チケット一覧データを読み込み、UIに反映します。
        /// </summary>
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
                        if (btnView != null)
                        {
                            btnView.clicked += () => GoToTicketDetail(t.Id);
                        }

                        _scvTicketList.Add(ticketEl);
                        
                        var spacer = new VisualElement();
                        spacer.style.height = 8;
                        _scvTicketList.Add(spacer);
                    }
                }
            });
        }

        /// <summary>
        /// チケット作成画面へ遷移します。
        /// </summary>
        private void GoToCreateTicket()
        {
            // AppFlowManagerに遷移を依頼
            AppFlowManager.Instance.GoToTicketCreate();
        }

        /// <summary>
        /// 指定されたチケットの詳細画面へ遷移します。
        /// </summary>
        /// <param name="ticketId">対象チケットのID</param>
        private void GoToTicketDetail(string ticketId)
        {
            // IDを指定してAppFlowManagerに遷移を依頼
            AppFlowManager.Instance.GoToTicketDetail(ticketId);
        }
    }
}