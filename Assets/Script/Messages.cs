using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Messages : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public TextAsset privateKeyAsset;

    private List<string> messages;
    private List<SpecialMessage> specialMessages;
    private List<string> welcomeMessages;

    private static bool firstMessageShown = false;
    private static bool secondMessageShown = false;

    private void Awake()
    {
/*        LoadMessages();
*/    }

    private void OnEnable()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        if (textMeshPro != null)
        {
            string messageToShow = GetRandomMessage();
            textMeshPro.text = messageToShow;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is not assigned or found.");
        }
    }

    /*private void LoadMessages()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("messages");
        MessageData messageData = JsonUtility.FromJson<MessageData>(jsonText.text);

        string privateKey = privateKeyAsset.text;

        messages = new List<string>();
        foreach (var message in messageData.messages)
        {
            messages.Add(Decrypt(message, privateKey));
        }

        specialMessages = new List<SpecialMessage>();
        foreach (var specialMessage in messageData.specialMessages)
        {
            specialMessages.Add(new SpecialMessage
            {
                message = Decrypt(specialMessage.message, privateKey),
                chance = specialMessage.chance
            });
        }

        welcomeMessages = new List<string>();
        foreach (var welcomeMessage in messageData.welcomeMessages)
        {
            welcomeMessages.Add(Decrypt(welcomeMessage, privateKey));
        }
    }*/

    private string GetRandomMessage()
    {
        if (!firstMessageShown)
        {
            firstMessageShown = true;
            return welcomeMessages[0];
        }

        if (!secondMessageShown)
        {
            secondMessageShown = true;
            return welcomeMessages[1];
        }

        // Check for special messages
        foreach (var specialMessage in specialMessages)
        {
            if (Random.value < specialMessage.chance)
            {
                return specialMessage.message;
            }
        }

        // If no special message is selected, choose a random regular message
        int randomIndex = Random.Range(0, messages.Count);
        return messages[randomIndex];
    }

    /*private string Decrypt(string cipherText, string privateKey)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(cipherText);
        byte[] decryptedData;

        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
            decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
        }

        return Encoding.UTF8.GetString(decryptedData);
    }*/

    [System.Serializable]
    private class MessageData
    {
        public string[] messages;
        public SpecialMessage[] specialMessages;
        public string[] welcomeMessages;
    }

    [System.Serializable]
    private class SpecialMessage
    {
        public string message;
        public float chance;
    }
}
