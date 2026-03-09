using System;
using UnityEngine;
using UnityEngine.UIElements;
using AIRegistry.Network;

namespace AIRegistry.Frontend.Controllers
{
    /// <summary>
    /// ログイン画面とユーザー登録画面のロジックを管理するプレゼンタークラスです。
    /// </summary>
    public class LoginPresenter : MonoBehaviour
    {
        private VisualElement _root;
        private TextField _txtUsername;
        private TextField _txtPassword;
        private Button _btnLogin;
        private Button _btnShowRegister;
        private Label _lblError;

        private VisualElement _modalRegister;
        private TextField _txtRegUsername;
        private TextField _txtRegPassword;
        private Button _btnCancelRegister;
        private Button _btnRegister;
        private Label _lblRegError;

        /// <summary>
        /// ログイン画面の初期化処理を行い、UI要素の参照とイベント登録を完了させます。
        /// </summary>
        /// <param name="root">画面のルート要素</param>
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

        /// <summary>
        /// オブジェクトが無効になった際のイベント解除等、クリーンアップを行います。
        /// </summary>
        private void OnDisable()
        {
            if (_btnLogin != null) _btnLogin.clicked -= OnLoginClicked;
            if (_btnShowRegister != null) _btnShowRegister.clicked -= ShowRegisterModal;
            if (_btnCancelRegister != null) _btnCancelRegister.clicked -= HideRegisterModal;
            if (_btnRegister != null) _btnRegister.clicked -= OnRegisterClicked;
        }

        /// <summary>
        /// ログインボタンがクリックされた際の認証処理です。
        /// </summary>
        private void OnLoginClicked()
        {
            string username = _txtUsername.value;
            string password = _txtPassword.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _lblError.text = "ユーザー名とパスワードを入力してください。";
                return;
            }

            _lblError.text = "ログイン中...";
            _btnLogin.SetEnabled(false);

            ApiClient.Instance.Login(username, password, (success, user) =>
            {
                _btnLogin.SetEnabled(true);
                if (success)
                {
                    _lblError.text = "";
                    // AppFlowManagerに遷移を依頼
                    AppFlowManager.Instance.GoToTicketList();
                }
                else
                {
                    _lblError.text = "ログインに失敗しました。";
                }
            });
        }

        /// <summary>
        /// ユーザー登録用のモーダルを表示します。
        /// </summary>
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

        /// <summary>
        /// ユーザー登録用モーダルを非表示にします。
        /// </summary>
        private void HideRegisterModal()
        {
            if (_modalRegister != null) _modalRegister.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// ユーザー登録実行ボタンが押された際の処理です。
        /// </summary>
        private void OnRegisterClicked()
        {
            string username = _txtRegUsername.value;
            string password = _txtRegPassword.value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _lblRegError.text = "すべての項目を入力してください。";
                return;
            }

            _lblRegError.text = "登録中...";
            _btnRegister.SetEnabled(false);

            ApiClient.Instance.Register(username, password, (success, user) =>
            {
                _btnRegister.SetEnabled(true);
                if (success)
                {
                    // 登録成功後もAppFlowManagerに遷移を依頼
                    AppFlowManager.Instance.GoToTicketList();
                }
                else
                {
                    _lblRegError.text = "登録に失敗しました。";
                }
            });
        }
    }
}