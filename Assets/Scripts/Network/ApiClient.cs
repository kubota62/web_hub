using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AIRegistry.Models;

namespace AIRegistry.Network
{
    /// <summary>
    /// 外部APIサーバーとの通信、または開発用のモックデータ提供を管理するクラスです。
    /// MonoBehaviourを継承し、シングルトンパターンで実装されています。
    /// </summary>
    public class ApiClient : MonoBehaviour
    {
        /// <summary> シングルトンインスタンスへのアクセスポイント </summary>
        public static ApiClient Instance { get; private set; }

        [Header("Settings")]
        [Tooltip("通信先のベースURL (XAMPPサーバーなど)")]
        [SerializeField] private string _baseUrl = "http://localhost/api";

        [Tooltip("trueの場合、ネットワーク通信を行わずモックデータを返します")]
        [SerializeField] private bool _useMockData = true;

        /// <summary>
        /// インスタンスの初期化とシングルトンの確立を行います。
        /// 既にインスタンスが存在する場合は重複を排除します。
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 新規ユーザー登録をリクエストします。
        /// </summary>
        /// <param name="username">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <param name="onCompleted">完了時のコールバック。(成功したか, 生成されたユーザー情報)</param>
        public void Register(string username, string password, Action<bool, User> onCompleted)
        {
            if (_useMockData)
            {
                // ネットワーク遅延をシミュレート
                StartCoroutine(SimulateDelay(() => {
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        var mockUser = new User { Id = "u_" + Guid.NewGuid().ToString(), Username = username };
                        onCompleted?.Invoke(true, mockUser);
                    }
                    else
                    {
                        onCompleted?.Invoke(false, null);
                    }
                }));
                return;
            }

            // 本番環境用の実装（POST /register など）はここに記述します
        }

        /// <summary>
        /// 既存ユーザーでのログインを試行します。
        /// </summary>
        /// <param name="username">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <param name="onCompleted">完了時のコールバック。(成功したか, ログインユーザー情報)</param>
        public void Login(string username, string password, Action<bool, User> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    var mockUser = new User { Id = "u_1", Username = username };
                    onCompleted?.Invoke(true, mockUser);
                }));
                return;
            }

            // 本番環境用の実装（POST /login など）はここに記述します
        }

        /// <summary>
        /// チケット（レビュー依頼）の一覧を取得します。
        /// </summary>
        /// <param name="onCompleted">完了時のコールバック。(成功したか, チケットのリスト)</param>
        public void GetTickets(Action<bool, List<ReviewTicket>> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    var mockTickets = new List<ReviewTicket>
                    {
                        new ReviewTicket { Id = "t_1", Title = "通信クラスのリファクタリング", Status = "Open", CreatedAt = DateTime.Now.ToString("yyyy-MM-dd") },
                        new ReviewTicket { Id = "t_2", Title = "ログイン画面UIの追加", Status = "Reviewed", CreatedAt = DateTime.Now.ToString("yyyy-MM-dd") }
                    };
                    onCompleted?.Invoke(true, mockTickets);
                }));
                return;
            }
            
            // 本番環境用の実装例:
            // StartCoroutine(GetRequest<List<ReviewTicket>>($"{_baseUrl}/tickets", onCompleted));
        }

        /// <summary>
        /// 新しいチケットをサーバー（またはモック）に作成します。
        /// </summary>
        /// <param name="newTicket">作成するチケットの情報</param>
        /// <param name="onCompleted">完了時のコールバック。(成功したか, IDが付番された後のチケット情報)</param>
        public void CreateTicket(ReviewTicket newTicket, Action<bool, ReviewTicket> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    newTicket.Id = "t_" + UnityEngine.Random.Range(100, 999);
                    newTicket.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd");
                    newTicket.Status = "Open";
                    onCompleted?.Invoke(true, newTicket);
                }));
                return;
            }
        }

        /// <summary>
        /// 指定されたチケットの詳細情報を取得します。
        /// </summary>
        /// <param name="ticketId">チケットID</param>
        /// <param name="onCompleted">完了時のコールバック</param>
        public void GetTicketDetail(string ticketId, Action<bool, ReviewTicket> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    var mock = new ReviewTicket
                    {
                        Id = ticketId,
                        Title = $"チケット詳細 ({ticketId})",
                        Status = "Open",
                        Description = "このチケットは、新しい通信レイヤーの追加に関する依頼です。",
                        DiffContent = "--- a/old_file.cs\n+++ b/new_file.cs\n@@ -1,3 +1,4 @@\n+ using UnityEngine.Networking;\n- using System.Net;",
                        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                    };
                    onCompleted?.Invoke(true, mock);
                }));
                return;
            }
            // TODO: 本番環境用のAPIリクエスト
            // StartCoroutine(GetRequest<ReviewTicket>($"{_baseUrl}/tickets/{ticketId}", onCompleted));
        }

        /// <summary>
        /// 指定されたチケットに対するAIレビュー結果を取得します。
        /// </summary>
        /// <param name="ticketId">対象チケットID</param>
        /// <param name="onCompleted">完了時のコールバック</param>
        public void GetAiReviewResult(string ticketId, Action<bool, AiReviewResult> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    var mock = new AiReviewResult
                    {
                        TicketId = ticketId,
                        Summary = "コードは概ね良好ですが、UnityWebRequest が適切に破棄されているか確認してください。",
                    };
                    onCompleted?.Invoke(true, mock);
                }));
                return;
            }
            // TODO: 本番環境用のAPIリクエスト
            // StartCoroutine(GetRequest<AiReviewResult>($"{_baseUrl}/tickets/{ticketId}/review", onCompleted));
        }

        // --- ネットワーク補助メソッド ---
        
        /// <summary>
        /// ネットワークの遅延を擬似的に発生させるコルーチンです（デバッグ・開発用）。
        /// </summary>
        private IEnumerator SimulateDelay(Action onComplete)
        {
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }

        /// <summary>
        /// ジェネリクスを用いた汎用的なGETリクエスト実行コルーチンです。
        /// JSONのパースも含めて処理します。
        /// </summary>
        /// <typeparam name="T">パース先のデータ型</typeparam>
        /// <param name="uri">リクエスト先URL</param>
        /// <param name="onCompleted">完了時のコールバック</param>
        private IEnumerator GetRequest<T>(string uri, Action<bool, T> onCompleted)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error GET: {webRequest.error}");
                    onCompleted?.Invoke(false, default);
                }
                else
                {
                    try
                    {
                        T responseData = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                        onCompleted?.Invoke(true, responseData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"JSON Parse Error: {e.Message}");
                        onCompleted?.Invoke(false, default);
                    }
                }
            }
        }
    }
}