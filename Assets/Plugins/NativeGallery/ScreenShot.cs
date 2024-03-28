using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenShot : MonoBehaviour
{
    // Start is called before the first frame update

    void Update()
    {
       
    }
    public void TakeScreenShot()
    {
        StartCoroutine(TakeScreenshotAndSave());
    }
    // Example code doesn't use this function but it is here for reference. It's recommended to ask for permissions manually using the
    // RequestPermissionAsync methods prior to calling NativeGallery functions
    private async void RequestPermissionAsynchronously(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
    {
        NativeGallery.Permission permission = await NativeGallery.RequestPermissionAsync(permissionType, mediaTypes);
        Debug.Log("Permission result: " + permission);
    }

    private IEnumerator TakeScreenshotAndSave()
    {
        this.GetComponent<Image>().enabled = false;
        yield return new WaitForEndOfFrame();

        
       




        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);

        // To avoid memory leaks
        Destroy(ss);
        //yield return new WaitForEndOfFrame();
        this.GetComponent<Image>().enabled = true;
    }



    
}
