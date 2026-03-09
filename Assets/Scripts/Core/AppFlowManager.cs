using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Frontend.Controllers;

namespace AIRegistry.Frontend
{
    /// <summary>
    /// アプリケーション全体の画面遷移（フロー）を管理する司令塔クラスです。
    /// 全てのプレゼンターの参照を保持し、画面の切り替えと初期化を一括して行います。
    /// </summary>
    public class AppFlowManager : MonoBehaviour
    {
        public static AppFlowManager Instance { get; private set; }

        [Header("UI Document References")]
        [SerializeField] private UIDocument _mainDocument;

        [Header("View Assets (UXML)")]
        [SerializeField] private VisualTreeAsset _loginViewAsset;
        [SerializeField] private VisualTreeAsset _ticketListViewAsset;
        [SerializeField] private VisualTreeAsset _ticketCreateViewAsset;
        [SerializeField] private VisualTreeAsset _ticketDetailViewAsset;

        [Header("Presenter References")]
        [Tooltip("Inspectorで各Presenterを紐付けてください。未指定の場合はAwakeで検索します")]
        [SerializeField] private LoginPresenter _loginPresenter;
        [SerializeField] private TicketListPresenter _listPresenter;
        [SerializeField] private TicketCreatePresenter _createPresenter;
        [SerializeField] private TicketDetailPresenter _detailPresenter;

        private VisualElement _mainContentContainer;

        /// <summary>
        /// インスタンスの初期化とシングルトンの確立、および各画面プレゼンターのキャッシュを行います。
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                CachePresenters();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 各Presenterの参照をキャッシュします。
        /// </summary>
        private void CachePresenters()
        {
            if (_loginPresenter == null) _loginPresenter = GetComponent<LoginPresenter>() ?? FindObjectOfType<LoginPresenter>();
            if (_listPresenter == null) _listPresenter = GetComponent<TicketListPresenter>() ?? FindObjectOfType<TicketListPresenter>();
            if (_createPresenter == null) _createPresenter = GetComponent<TicketCreatePresenter>() ?? FindObjectOfType<TicketCreatePresenter>();
            if (_detailPresenter == null) _detailPresenter = GetComponent<TicketDetailPresenter>() ?? FindObjectOfType<TicketDetailPresenter>();
        }

        /// <summary>
        /// アプリケーション起動時の初期フローを開始します。
        /// </summary>
        private void Start()
        {
            InitializeFlow();
        }

        /// <summary>
        /// UIドキュメントの準備ができ次第、初期表示画面（ログイン）の設定を行います。
        /// </summary>
        private void InitializeFlow()
        {
            if (_mainDocument == null) return;
            var root = _mainDocument.rootVisualElement;
            if (root == null)
            {
                Invoke(nameof(InitializeFlow), 0.1f);
                return;
            }

            _mainContentContainer = root.Q<VisualElement>("MainContent");
            if (_mainContentContainer != null)
            {
                // 最初のログイン画面を表示
                GoToLogin();
            }
        }

        // --- 画面遷移メソッド群 ---

        /// <summary>
        /// ログイン画面へ遷移し、ログインプレゼンターを初期化します。
        /// </summary>
        public void GoToLogin()
        {
            ShowView(_loginViewAsset);
            _loginPresenter?.Initialize(_mainContentContainer);
        }

        /// <summary> チケット一覧画面へ遷移します </summary>
        public void GoToTicketList()
        {
            ShowView(_ticketListViewAsset);
            _listPresenter?.Initialize(_mainContentContainer);
        }

        /// <summary> 新規チケット作成画面へ遷移します </summary>
        public void GoToTicketCreate()
        {
            ShowView(_ticketCreateViewAsset);
            _createPresenter?.Initialize(_mainContentContainer);
        }

        /// <summary> チケット詳細画面へ遷移します </summary>
        /// <param name="ticketId">表示するチケットのID</param>
        public void GoToTicketDetail(string ticketId)
        {
            ShowView(_ticketDetailViewAsset);
            _detailPresenter?.Initialize(_mainContentContainer, ticketId);
        }

        /// <summary>
        /// コンテナの中身を入れ替え、共通ヘッダーのバインドを行います。
        /// </summary>
        private void ShowView(VisualTreeAsset viewAsset)
        {
            if (_mainContentContainer == null || viewAsset == null) return;
            _mainContentContainer.Clear();
            var currentView = viewAsset.Instantiate();
            currentView.style.flexGrow = 1;
            _mainContentContainer.Add(currentView);

            BindGlobalHeader(currentView);
        }

        /// <summary>
        /// 各画面に共通して存在するヘッダーボタン（Home, NewTicket, Logout）に対して
        /// AppFlowManagerの画面遷移処理をバインドします。
        /// </summary>
        /// <param name="viewRoot">遷移先の画面ルート要素</param>
        private void BindGlobalHeader(VisualElement viewRoot)
        {
            var btnHome = viewRoot.Q<Button>("BtnHome");
            var btnNewTicket = viewRoot.Q<Button>("BtnNewTicket");
            var btnLogout = viewRoot.Q<Button>("BtnLogout");

            if (btnHome != null) btnHome.clicked += GoToTicketList;
            if (btnNewTicket != null) btnNewTicket.clicked += GoToTicketCreate;
            if (btnLogout != null) btnLogout.clicked += GoToLogin;
        }
    }
}