using UnityEngine;

public class TextureManager : GenericSingleton<TextureManager> {
	public GameObject ZhiBookImage;
	public GameObject DogBookImage;
	
	private void Awake() {
		ZhiBookImage.SetActive(false);
		DogBookImage.SetActive(false);
	}
    
	public void ActivateBook(PlayerType type) {
		if (type == PlayerType.Human) {
			ZhiBookImage.SetActive(true);
		}else if (type == PlayerType.Ghost) {
			DogBookImage.SetActive(true);
		}
	}
	public void DeactivateBook(PlayerType type) {
		if (type == PlayerType.Human) {
			ZhiBookImage.SetActive(false);
		}else if (type == PlayerType.Ghost) {
			DogBookImage.SetActive(false);
		}
	}

}