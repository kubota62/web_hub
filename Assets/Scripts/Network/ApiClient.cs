using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AIRegistry.Models;

namespace AIRegistry.Network
{
    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }

        [SerializeField] private string _baseUrl = "http://localhost/api"; // XAMPP server URL
        [SerializeField] private bool _useMockData = true; // Use mock data for initial development

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

        public void Register(string username, string password, Action<bool, User> onCompleted)
        {
            if (_useMockData)
            {
                // Simulate network delay
                StartCoroutine(SimulateDelay(() => {
                    // For mock, any non-empty credential succeeds
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

            // Real implementation would go here (e.g. POST /register)
        }

        public void Login(string username, string password, Action<bool, User> onCompleted)
        {
            if (_useMockData)
            {
                // Simulate network delay
                StartCoroutine(SimulateDelay(() => {
                    var mockUser = new User { Id = "u_1", Username = username };
                    onCompleted?.Invoke(true, mockUser);
                }));
                return;
            }

            // Real implementation would go here (e.g. POST /login)
        }

        public void GetTickets(Action<bool, List<ReviewTicket>> onCompleted)
        {
            if (_useMockData)
            {
                StartCoroutine(SimulateDelay(() => {
                    var mockTickets = new List<ReviewTicket>
                    {
                        new ReviewTicket { Id = "t_1", Title = "Refactor network class", Status = "Open", CreatedAt = DateTime.Now.ToString("yyyy-MM-dd") },
                        new ReviewTicket { Id = "t_2", Title = "Add login UI", Status = "Reviewed", CreatedAt = DateTime.Now.ToString("yyyy-MM-dd") }
                    };
                    onCompleted?.Invoke(true, mockTickets);
                }));
                return;
            }
            
            // Example real request:
            // StartCoroutine(GetRequest<List<ReviewTicket>>($"{_baseUrl}/tickets", onCompleted));
        }

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

        // --- Network Helper Methods ---
        
        private IEnumerator SimulateDelay(Action onComplete)
        {
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();
        }

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