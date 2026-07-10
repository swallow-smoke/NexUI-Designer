using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace emiteat.NexUI.Integrations.Figma
{
    /// <summary>Deserialized response of Figma's <c>GET /v1/me</c> - used only to confirm a token is valid.</summary>
    [Serializable]
    public sealed class FigmaUser
    {
        public string id;
        public string email;
        public string handle;
    }

    /// <summary>
    /// C5 (first slice): thin wrapper over the Figma REST API's authentication surface.
    /// Confirms a personal access token works and can pull a file's raw JSON document. The
    /// frame -&gt; NexUI element mapping (Auto Layout conversion, text layers, coordinate
    /// conversion, nested components, auto-binding by name) is deliberately out of scope for
    /// this slice - Figma's node JSON is a deeply recursive, polymorphic tree that needs a
    /// dedicated parser/mapper, not <see cref="JsonUtility"/>. This class only proves the
    /// connection works so that mapper has something real to build on.
    /// </summary>
    public static class FigmaApiClient
    {
        private const string BaseUrl = "https://api.figma.com/v1";

        /// <summary>Calls GET /v1/me. Throws if the token is missing/invalid or the request fails.</summary>
        public static async UniTask<FigmaUser> GetAuthenticatedUserAsync(string token)
        {
            using var request = UnityWebRequest.Get($"{BaseUrl}/me");
            request.SetRequestHeader("X-Figma-Token", token);
            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException($"Figma API error ({request.responseCode}): {request.error}");

            return JsonUtility.FromJson<FigmaUser>(request.downloadHandler.text);
        }

        /// <summary>Calls GET /v1/files/{fileKey} and returns the raw JSON document (the file's full node tree).</summary>
        public static async UniTask<string> GetFileJsonAsync(string token, string fileKey)
        {
            if (string.IsNullOrEmpty(fileKey))
                throw new ArgumentException("Figma file key is required.", nameof(fileKey));

            using var request = UnityWebRequest.Get($"{BaseUrl}/files/{fileKey}");
            request.SetRequestHeader("X-Figma-Token", token);
            await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException($"Figma API error ({request.responseCode}): {request.error}");

            return request.downloadHandler.text;
        }
    }
}
