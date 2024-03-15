#ifndef __G_UI_H__
#define __G_UI_H__

#include "Application.h"

#include "Globals.h"
#include "GameObject.h"
#include "C_UI.h"
#include "CTransform.h"

#include "ModuleScene.h"

class UI_Canvas;

class G_UI : public GameObject
{
public:
	G_UI(UI_TYPE t, GameObject* pParent = External->scene->mRootNode, float x = 0, float y = 0);
	~G_UI();

	update_status Update(float dt);
	C_UI* GetComponentUI(UI_TYPE type);
	std::vector<C_UI*> GetComponentsUI_ByType(UI_TYPE type);

	bool AddUIComponent(UI_TYPE type, float x, float y, GameObject* parent = External->scene->mRootNode);

public:
	UI_Canvas* canvas;
	
	//CTransform rectTransform;
};
#endif // __G_UI_H__