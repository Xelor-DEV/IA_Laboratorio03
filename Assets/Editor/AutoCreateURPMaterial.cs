using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

public class CreateURPMaterialEditor : Editor
{
    [MenuItem("Tools/Create URP Material From Texture %#m")]
    private static void CreateMaterialFromTexture()
    {
        // Obtener la textura seleccionada
        Texture2D selectedTexture = Selection.activeObject as Texture2D;

        if (selectedTexture == null)
        {
            Debug.LogError("¡No hay ninguna textura seleccionada!");
            return;
        }

        // Crear el material URP
        CreateURPMaterial(selectedTexture);
    }

    private static void CreateURPMaterial(Texture2D texture)
    {
        // Configurar el shader URP
        Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");

        if (urpShader == null)
        {
            Debug.LogError("¡Shader URP/Lit no encontrado!");
            return;
        }

        // Crear nuevo material
        Material newMaterial = new Material(urpShader)
        {
            name = texture.name + "_Material"
        };

        // Asignar la textura al material
        newMaterial.SetTexture("_BaseMap", texture);

        // Guardar el material
        string texturePath = AssetDatabase.GetAssetPath(texture);
        string materialPath = Path.ChangeExtension(texturePath, ".mat");
        materialPath = AssetDatabase.GenerateUniqueAssetPath(materialPath);

        AssetDatabase.CreateAsset(newMaterial, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Seleccionar el material creado
        EditorGUIUtility.PingObject(newMaterial);
        Selection.activeObject = newMaterial;

        Debug.Log($"Material URP creado: {materialPath}");
    }
}