using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PinShot.Singletons {
    public class SaveDataManager : BaseSingleton<SaveDataManager> {
        // 暗号化のためのキーとIV（初期化ベクトル）
        private string _encryptionKey;  // 32バイト
        private string _encryptionIV;   // 16バイト

        // 暗号化を使用するかどうか
        [SerializeField] private bool _useEncryption = true;

        protected override void Initialize() {
            base.Initialize();

            // デバイス固有の情報からキーを生成（より安全）
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            _encryptionKey = GenerateKey(deviceId, 32);
            _encryptionIV = GenerateKey(ReverseString(deviceId), 16);
        }

        /// <summary>
        /// 文字列を反転するヘルパーメソッド
        /// </summary>
        private string ReverseString(string input) {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        /// <summary>
        /// デバイスIDから暗号化キーを生成
        /// </summary>
        private string GenerateKey(string seed, int length) {
            // シードからキーを生成
            using (SHA256 sha = SHA256.Create()) {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(seed + "PinShot_Salt"));
                return Convert.ToBase64String(hash).Substring(0, length);
            }
        }

        /// <summary>
        /// 文字列を暗号化します
        /// </summary>
        /// <param name="plainText">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        private string Encrypt(string plainText) {
            try {
                if (string.IsNullOrEmpty(plainText)) return plainText;
                if (string.IsNullOrEmpty(_encryptionKey) || string.IsNullOrEmpty(_encryptionIV)) {
                    Debug.LogWarning("暗号化キーが初期化されていません。暗号化をスキップします。");
                    return plainText;
                }

                using (Aes aes = Aes.Create()) {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = Encoding.UTF8.GetBytes(_encryptionIV);
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var msEncrypt = new System.IO.MemoryStream()) {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                            using (var swEncrypt = new System.IO.StreamWriter(csEncrypt)) {
                                swEncrypt.Write(plainText);
                            }
                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }
            catch (CryptographicException ce) {
                Debug.LogError($"暗号化エラー: {ce.Message}");
                return plainText;
            }
            catch (Exception e) {
                Debug.LogError($"暗号化に失敗しました: {e.Message}");
                return plainText;
            }
        }

        /// <summary>
        /// 文字列を復号化します
        /// </summary>
        /// <param name="cipherText">復号化する暗号文</param>
        /// <returns>復号化された文字列</returns>
        private string Decrypt(string cipherText) {
            try {
                if (string.IsNullOrEmpty(cipherText)) return cipherText;
                if (string.IsNullOrEmpty(_encryptionKey) || string.IsNullOrEmpty(_encryptionIV)) {
                    Debug.LogWarning("暗号化キーが初期化されていません。復号化をスキップします。");
                    return cipherText;
                }

                // 暗号化されたテキストかどうかを簡易チェック
                if (!IsBase64String(cipherText)) {
                    Debug.LogWarning("暗号化されていないテキストの可能性があります。復号化をスキップします。");
                    return cipherText;
                }

                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create()) {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                    aes.IV = Encoding.UTF8.GetBytes(_encryptionIV);
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (var msDecrypt = new System.IO.MemoryStream(buffer)) {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                            using (var srDecrypt = new System.IO.StreamReader(csDecrypt)) {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (FormatException fe) {
                Debug.LogWarning($"Base64形式ではないデータです: {fe.Message}");
                return cipherText;
            }
            catch (CryptographicException ce) {
                Debug.LogError($"復号化エラー: {ce.Message}");
                return cipherText;
            }
            catch (Exception e) {
                Debug.LogError($"復号化に失敗しました: {e.Message}");
                return cipherText;
            }
        }

        /// <summary>
        /// 文字列がBase64形式かどうかを判定
        /// </summary>
        private bool IsBase64String(string base64) {
            // Base64文字列の簡易チェック
            try {
                // 長さチェック (Base64は4の倍数)
                if (base64.Length % 4 != 0) return false;

                // 使用可能文字チェック
                string base64Chars = base64.Replace("=", "");
                foreach (char c in base64Chars) {
                    if (!((c >= 'A' && c <= 'Z') ||
                          (c >= 'a' && c <= 'z') ||
                          (c >= '0' && c <= '9') ||
                          c == '+' || c == '/')) {
                        return false;
                    }
                }
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// オブジェクトをJSON形式でPlayerPrefsに保存
        /// </summary>
        /// <param name="key">保存するキー</param>
        /// <param name="data">保存するデータのインスタンス</param>
        /// <typeparam name="T"></typeparam>
        public void Save<T>(string key, T data) where T : class {
            try {
                string json = JsonUtility.ToJson(data);
                if (_useEncryption) {
                    json = Encrypt(json);
                }
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
                Debug.Log($"データを保存しました: {key} {(_useEncryption ? "🔒(暗号化)" : "")}");
            }
            catch (Exception e) {
                Debug.LogError($"セーブに失敗しました。Key: {key}, Error: {e.Message}");
            }
        }

        /// <summary>
        /// PlayerPrefsからJSONデータを読み込みオブジェクトに変換
        /// </summary>
        /// <param name="key">読み込むデータのキー</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string key) where T : class {
            try {
                if (!PlayerPrefs.HasKey(key)) {
                    Debug.Log($"データが見つかりません: {key}");
                    return null;
                }

                string json = PlayerPrefs.GetString(key);
                if (_useEncryption) {
                    json = Decrypt(json);
                }
                T data = JsonUtility.FromJson<T>(json);
                Debug.Log($"データを読み込みました: {key} {(_useEncryption ? "🔒(暗号化)" : "")}");
                return data;
            }
            catch (Exception e) {
                Debug.LogError($"ロードに失敗しました。Key: {key}, Error: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 保存したデータを削除
        /// </summary>
        /// <param name="key">削除するデータのキー</param>
        public void Delete(string key) {
            if (PlayerPrefs.HasKey(key)) {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
                Debug.Log($"データを削除しました: {key}");
            }
            else {
                Debug.Log($"削除するデータが見つかりません: {key}");
            }
        }

        /// <summary>
        /// 全てのセーブデータを削除
        /// </summary>
        public void DeleteAll() {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("全てのデータを削除しました");
        }

        /// <summary>
        /// 指定したキーのデータが存在するか確認します
        /// </summary>
        /// <param name="key">確認するキー</param>
        /// <returns>データが存在すればtrue</returns>
        public bool HasData(string key) {
            return PlayerPrefs.HasKey(key);
        }
    }
}
