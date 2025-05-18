using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// ビルド後にライセンスファイルをビルドフォルダにコピーするクラス
/// </summary>
public class PostBuildProcessor : IPostprocessBuildWithReport {
    public int callbackOrder => 0;

    // ビルド後処理
    public void OnPostprocessBuild(BuildReport report) {
        string buildPath = report.summary.outputPath;
        string buildDirectory = Path.GetDirectoryName(buildPath);

        // LICENSE.md のコピー
        CopyLicenseFile(buildDirectory);

        Debug.Log($"ライセンスファイルをコピーしました: {buildDirectory}");
    }

    // ライセンスファイルをコピーする
    private void CopyLicenseFile(string buildDirectory) {
        string sourceFilePath = Path.Combine(Application.dataPath, "..", "LICENSE.md");
        string destFilePath = Path.Combine(buildDirectory, "LICENSE.md");

        if (File.Exists(sourceFilePath)) {
            File.Copy(sourceFilePath, destFilePath, true);
        }
        else {
            Debug.LogWarning($"ライセンスファイルが見つかりません: {sourceFilePath}");
        }
    }
}
