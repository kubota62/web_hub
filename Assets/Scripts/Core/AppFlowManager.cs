using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Frontend.Controllers;

namespace AIRegistry.Frontend
{
    public class AppFlowManager : MonoBehaviour
    {
        public static AppFlowManager Instance { get; private set; }

        [SerializeField] private UIDocument _mainDocument;
        [SerializeField] private VisualTreeAsset _initialView;

        private VisualElement _mainContentContainer;
        private VisualElement _currentView;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeFlow();
        }

        private void InitializeFlow()
        {
            if (_mainDocument == null)
            {
                Debug.LogError("AppFlowManager: UIDocument is not assigned.");
                return;
            }

            var root = _mainDocument.rootVisualElement;
            if (root == null)
            {
                // If not ready yet, try again in the next frame
                Invoke(nameof(InitializeFlow), 0.1f);
                return;
            }

            _mainContentContainer = root.Q<VisualElement>("MainContent");

            if (_mainContentContainer == null)
            {
                Debug.LogError("AppFlowManager: Could not find 'MainContent' container in UXML. Check if MainView.uxml has an element named 'MainContent'.");
            }
            else if (_initialView != null)
            {
                ShowView(_initialView);
                
                // If the first view is LoginView, initialize LoginPresenter
                if (_initialView.name.Contains("Login"))
                {
                    var lp = GetComponent<LoginPresenter>();
                    if (lp != null) lp.Initialize(_mainContentContainer);
                }
            }
        }

        /// <summary>
        /// Loads and displays a new visual tree in the main content area.
        /// </summary>
        public void ShowView(VisualTreeAsset viewAsset)
        {
            if (_mainContentContainer == null || viewAsset == null) return;

            // Clear existing view
            _mainContentContainer.Clear();

            // Instantiate and add new view
            _currentView = viewAsset.Instantiate();
            _currentView.style.flexGrow = 1; // Ensure it fills the container
            _mainContentContainer.Add(_currentView);

            // 遷移後にヘッダーがあれば共通ボタンをバインドする
            BindGlobalHeader(_currentView);
        }

        public void BindGlobalHeader(VisualElement viewRoot)
        {
            var btnHome = viewRoot.Q<Button>("BtnHome");
            var btnNewTicket = viewRoot.Q<Button>("BtnNewTicket");
            var btnLogout = viewRoot.Q<Button>("BtnLogout");

            if (btnHome != null)
            {
                btnHome.clicked -= OnHomeClicked;
                btnHome.clicked += OnHomeClicked;
            }

            if (btnNewTicket != null)
            {
                btnNewTicket.clicked -= OnNewTicketClicked;
                btnNewTicket.clicked += OnNewTicketClicked;
            }
            
            if (btnLogout != null)
            {
                btnLogout.clicked -= OnLogoutClicked;
                btnLogout.clicked += OnLogoutClicked;
            }
        }

        private void OnHomeClicked()
        {
            var presenter = FindObjectOfType<TicketListPresenter>();
            if (presenter != null)
            {
                presenter.GoToList();
            }
        }

        private void OnNewTicketClicked()
        {
            var presenter = FindObjectOfType<TicketCreatePresenter>();
            if (presenter != null)
            {
                presenter.GoToCreate();
            }
        }

        private void OnLogoutClicked()
        {
            // ログアウト処理（ログイン画面へ戻す）
            if (_initialView != null)
            {
                ShowView(_initialView);
                var lp = FindObjectOfType<LoginPresenter>();
                if (lp != null) lp.Initialize(_mainContentContainer);
            }
        }
    }
}