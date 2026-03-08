using System;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;

namespace AIRegistry.Frontend.Controllers
{
    public class LoginPresenter : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _ticketListViewAsset;

        private VisualElement _root;
        private TextField _txtUsername;
        private TextField _txtPassword;
        private Button _btnLogin;
        private Button _btnShowRegister;
        private Label _lblError;

        // Registration Modal UI
        private VisualElement _modalRegister;
        private TextField _txtRegUsername;
        private TextField _txtRegPassword;
        private Button _btnCancelRegister;
        private Button _btnRegister;
        private Label _lblRegError;

        public void Initialize(VisualElement root)
        {
            _root = root;
            _txtUsername = _root.Q<TextField>("TxtUsername");
            _txtPassword = _root.Q<TextField>("TxtPassword");
            _btnLogin = _root.Q<Button>("BtnLogin");
            _btnShowRegister = _root.Q<Button>("BtnShowRegister");
            _lblError = _root.Q<Label>("LblError");

            _modalRegister = _root.Q<VisualElement>("ModalRegister");
            _txtRegUsername = _root.Q<TextField>("TxtRegUsername");
            _txtRegPassword = _root.Q<TextField>("TxtRegPassword");
            _btnCancelRegister = _root.Q<Button>("BtnCancelRegister");
            _btnRegister = _root.Q<Button>("BtnRegister");
            _lblRegError = _root.Q<Label>("LblRegError");

            if (_btnLogin != null)
            {
                _btnLogin.clicked -= OnLoginClicked;
                _btnLogin.clicked += OnLoginClicked;
            }
            else
                Debug.LogError("LoginPresenter: Login Button not found in XML!");

            if (_btnShowRegister != null)
            {
                _btnShowRegister.clicked -= ShowRegisterModal;
                _btnShowRegister.clicked += ShowRegisterModal;
            }

            if (_btnCancelRegister != null)
            {
                _btnCancelRegister.clicked -= HideRegisterModal;
                _btnCancelRegister.clicked += HideRegisterModal;
            }

            if (_btnRegister != null)
            {
                _btnRegister.clicked -= OnRegisterClicked;
                _btnRegister.clicked += OnRegisterClicked;
            }
        }

        private void OnDisable()
        {
            if (_btnLogin != null) _btnLogin.clicked -= OnLoginClicked;
            if (_btnShowRegister != null) _btnShowRegister.clicked -= ShowRegisterModal;
            if (_btnCancelRegister != null) _btnCancelRegister.clicked -= HideRegisterModal;
            if (_btnRegister != null) _btnRegister.clicked -= OnRegisterClicked;
        }

        private void OnLoginClicked()
        {
            string username = _txtUsername.value;
            string password = _txtPassword.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _lblError.text = "Username and password cannot be empty.";
                return;
            }

            _lblError.text = "Logging in...";
            _btnLogin.SetEnabled(false);

            ApiClient.Instance.Login(username, password, (success, user) =>
            {
                _btnLogin.SetEnabled(true);
                if (success)
                {
                    _lblError.text = "";
                    Debug.Log($"Logged in as {user.Username}!");
                    
                    // AppFlowManager経由で初期化済みのTicketListPresenterに遷移を依頼する
                    var listPresenter = FindObjectOfType<TicketListPresenter>();
                    if (listPresenter != null)
                    {
                        listPresenter.GoToList();
                    }
                }
                else
                {
                    _lblError.text = "Login failed. Invalid credentials.";
                }
            });
        }

        private void ShowRegisterModal()
        {
            if (_modalRegister != null)
            {
                _modalRegister.style.display = DisplayStyle.Flex;
                _txtRegUsername.value = "";
                _txtRegPassword.value = "";
                _lblRegError.text = "";
            }
        }

        private void HideRegisterModal()
        {
            if (_modalRegister != null)
            {
                _modalRegister.style.display = DisplayStyle.None;
            }
        }

        private void OnRegisterClicked()
        {
            string username = _txtRegUsername.value;
            string password = _txtRegPassword.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _lblRegError.text = "Username and password cannot be empty.";
                return;
            }

            _lblRegError.text = "Registering...";
            _btnRegister.SetEnabled(false);

            ApiClient.Instance.Register(username, password, (success, user) =>
            {
                _btnRegister.SetEnabled(true);
                if (success)
                {
                    Debug.Log($"Successfully registered and logged in as {user.Username}!");
                    FindObjectOfType<TicketListPresenter>()?.GoToList();
                }
                else
                {
                    _lblRegError.text = "Registration failed. Try different credentials.";
                }
            });
        }
    }
}