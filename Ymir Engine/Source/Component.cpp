#include "Component.h"
#include "GameObject.h"

#include "ModuleEditor.h"
#include "ModuleScene.h"

#include "External/mmgr/mmgr.h"

Component::Component()
{
	mOwner = nullptr;
	ctype = ComponentType::NONE;

	active = true;

	UID = Random::Generate();
}

Component::Component(GameObject* owner, ComponentType type)
{
	mOwner = owner;
	ctype = type;

	active = true;

	UID = Random::Generate();
}

Component::~Component()
{
	mOwner = nullptr;
}

void Component::Enable()
{
	active = true;
}

void Component::Disable()
{
	active = false;
}

void Component::Update()
{

}

void Component::OnInspector()
{

}

uint Component::GetUID()
{
	return UID;
}

GameObject* Component::ImGui_GameObjectReference(GameObject* go, bool* buttonClicked)
{
	ImGui::PushStyleColor(ImGuiCol_Button, ENGINE_COLOR);
	ImGui::PushStyleColor(ImGuiCol_ButtonHovered, ENGINE_COLOR);
	if (ImGui::Button(go == nullptr ? "<null>" : (go)->name.c_str()))
	{
		if (buttonClicked != nullptr)
		{
			*buttonClicked = true;
		}

		if (go != nullptr)
		{
			External->scene->SetSelected(go);
		}
	}

	if (ImGui::BeginDragDropTarget())
	{
		if (const ImGuiPayload* payload = ImGui::AcceptDragDropPayload("GameObject"))
		{
			if (go != nullptr)
			{
				go->RemoveReference(this);
			}
			go = External->editor->draggedGO;
			go->vReferences.push_back(this);
		}
		ImGui::EndDragDropTarget();
	}
	ImGui::PopStyleColor(2);

	return go;
}

void Component::SetReference(GameObject& pointer, GameObject& newPointer)
{
	if (&pointer != nullptr)
	{
		pointer.RemoveReference(this);
	}
	pointer = newPointer;
	pointer.vReferences.push_back(this);
}

void Component::OnReferenceDestroyed(void* ptr)
{
}
