#include "CMaterial.h"

#include "External/ImGui/imgui.h"
#include "External/ImGui/backends/imgui_impl_sdl2.h"
#include "External/ImGui/backends/imgui_impl_opengl3.h"

#include "Application.h"
#include "ModuleRenderer3D.h"
#include "GameObject.h"

CMaterial::CMaterial(GameObject* owner) : Component(owner, ComponentType::MATERIAL)
{

}

CMaterial::~CMaterial()
{

}

void CMaterial::Enable()
{

}

void CMaterial::Disable()
{

}

void CMaterial::Update()
{

}

void CMaterial::OnInspector()
{
    ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_DefaultOpen;

    if (ImGui::CollapsingHeader("Material", flags))
    {
        ImGui::Indent();

        ImGui::Image(reinterpret_cast<void*>(static_cast<intptr_t>(ID)), ImVec2(200,200));
        ImGui::Spacing();

        ImGui::Text("Path: %s", path.c_str());
        ImGui::Spacing();

        if (ImGui::Button("Apply Checker Texture")) {

            External->renderer3D->ApplyCheckerTexture();

        }

        ImGui::Spacing();

        if (ImGui::Button("Clear Actual Texture")) {

            External->renderer3D->ClearActualTexture();

        }

        ImGui::Spacing();

        ImGui::Unindent();
    }

}
