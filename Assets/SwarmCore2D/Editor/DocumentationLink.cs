using UnityEditor;
using UnityEngine;

namespace SwarmCore2D.Editor
{
    public static class DocumentationLink
    {
        const string DocsURL =
            "https://docs.google.com/document/d/10dqUg6zNqsQVJ4vbRJg33QB2lscMgJ9nmzHm3p-SEXQ/edit?usp=sharing";

        [MenuItem("Tools/SwarmCore2D/Open Documentation")]
        public static void OpenDocs()
        {
            Application.OpenURL(DocsURL);
        }
    }
}