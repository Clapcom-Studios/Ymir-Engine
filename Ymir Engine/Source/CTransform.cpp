#include "CTransform.h"

#include "External/ImGui/imgui.h"
#include "External/ImGui/backends/imgui_impl_sdl2.h"
#include "External/ImGui/backends/imgui_impl_opengl3.h"

#include "Application.h"
#include "ModuleRenderer3D.h"
#include "GameObject.h"

CTransform::CTransform(GameObject* owner) : Component(owner, ComponentType::TRANSFORM)
{
	translation = float3::zero;
	eulerRot = float3::zero;
	scale = float3::zero;

	translationPtr = nullptr;
	rotationPtr = nullptr;
	scalePtr = nullptr;

	resetPressed = false;

	mGlobalMatrix = float4x4::identity;

	// Load transform info from shader --------------- bad, component mesh doesn't exist before transform
	//meshComponent = static_cast<CMesh*>(owner->GetComponent(ComponentType::MESH));
	//if (meshComponent != nullptr)
	//{
	//	 translation = meshComponent->meshReference->meshShader.translation;
	//	 eulerRot = meshComponent->meshReference->meshShader.rotation;
	//	 scale = meshComponent->meshReference->meshShader.scale;
	//}

	dirty_ = true;

}

CTransform::CTransform(GameObject* g, float3 pos, Quat rot, float3 sc, bool start_enabled)
{
	translation = pos;
	rotation = rot;
	eulerRot = rotation.ToEulerXYZ();
	scale = sc;

	mGlobalMatrix = math::float4x4::FromTRS(pos, rot, sc);
	mLocalMatrix = math::float4x4::identity;

	resetPressed = false;

	dirty_ = true;
}

CTransform::~CTransform()
{ 
	// TODO: RELEASE(meshComponent);
}

void CTransform::Update()
{
	if (dirty_)
	{
		UpdateTransformsChilds();
	}
}

void CTransform::OnInspector()
{
	ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags_OpenOnArrow | ImGuiTreeNodeFlags_DefaultOpen;

	if (ImGui::CollapsingHeader("Transform", flags))
	{
		ImGui::Indent();

		/*for (auto it = External->renderer3D->models.begin(); it != External->renderer3D->models.end(); ++it) {

			for (auto jt = (*it).meshes.begin(); jt != (*it).meshes.end(); ++jt) {

				if ((*jt).meshGO->selected || (*it).modelGO->selected) {

					translation = (*jt).meshShader.translation;
					rotation = (*jt).meshShader.rotation;
					scale = (*jt).meshShader.scale;

					translationPtr = &(*jt).meshShader.translation;
					rotationPtr = &(*jt).meshShader.rotation;
					scalePtr = &(*jt).meshShader.scale;

					mGlobalMatrix = float4x4::FromTRS(*translationPtr, Quat::FromEulerXYZ((*rotationPtr).x * DEGTORAD, (*rotationPtr).y * DEGTORAD, (*rotationPtr).z * DEGTORAD).Normalized(), *scalePtr).Transposed();

					ImGui::DragFloat3("Transform", (*translationPtr).ptr(), 0.1f);
					ImGui::DragFloat3("Rotation", (*rotationPtr).ptr(), 0.1f);
					ImGui::DragFloat3("Scale", (*scalePtr).ptr(), 0.1f);

					if (ImGui::Button("Reset Transformations")) {

						(*jt).meshShader.Translate({ 0,0,0 });
						(*jt).meshShader.Rotate({ 0,0,0 });
						(*jt).meshShader.Scale({ 1,1,1 });

						resetPressed = true;

					}

				}

			}

		}*/

		if (ImGui::DragFloat3("Position", &translation[0], 0.1f))
		{
			SetPosition(translation);
		}

		if (ImGui::DragFloat3("Rotation", &eulerRot[0], 0.1f))
		{
			SetRotation(eulerRot);
		}

		if (ImGui::DragFloat3("Scale", &scale[0], 0.1f))
		{
			SetScale(scale);
		}

		ImGui::Unindent();
	}

}

void CTransform::SetTransform(float3 pos, float3 rot, float3 scale)
{
	SetPosition(pos);
	SetRotation(rot);
	SetScale(scale);
}

void CTransform::SetTransform(CTransform& t)
{
	SetPosition(t.translation);
	SetRotation(t.eulerRot);
	SetScale(t.scale);
}

void CTransform::SetPosition(float3 vec)
{
	translation = float3(vec);
	dirty_ = true;
}

void CTransform::SetRotation(float3 vec)
{
	rotation = Quat::FromEulerXYZ(vec[0] * DEGTORAD, vec[1] * DEGTORAD, vec[2] * DEGTORAD);
	dirty_ = true;
}

void CTransform::SetScale(float3 vec)
{
	scale = float3(vec);
	dirty_ = true;
}

float4x4 CTransform::GetGlobalTransform() const
{
	return mGlobalMatrix;
}

float4x4 CTransform::GetLocalTransform() const
{
	return mLocalMatrix;
}

float3 CTransform::GetGlobalPosition() const
{
	float3 pos, sc;
	Quat rot;
	mGlobalMatrix.Decompose(pos, rot, sc);

	return pos;
}

Quat CTransform::GetLocalRotation() const
{
	return rotation;
}

void CTransform::UpdateTransformsChilds()
{
	UpdateGlobalMatrix();

	if (!mOwner->mChildren.empty())
	{
		for (auto i = 0; i < mOwner->mChildren.size(); i++)
		{
			mOwner->mChildren[i]->mTransform->UpdateTransformsChilds();
		}
	}
	dirty_ = false;
}

void CTransform::UpdateGlobalMatrix()
{
	UpdateLocalMatrix();

	if (mOwner->mParent != nullptr && mOwner->mParent->mTransform != nullptr)
	{
		float4x4 Global_parent = mOwner->mParent->mTransform->mGlobalMatrix;
		mGlobalMatrix = Global_parent * mLocalMatrix;// Your global matrix = your parent�s global matrix * your local Matrix
	}

	else
	{
		mGlobalMatrix = mLocalMatrix;
	}

	UpdateBoundingBoxes();
}

void CTransform::UpdateLocalMatrix()
{
	mLocalMatrix = float4x4::FromTRS(translation, rotation, scale);
	eulerRot = rotation.ToEulerXYZ();
	eulerRot *= RADTODEG;

	// TODO: Check if mesh exists? Probably there is a better way to do it. Then, update shader values.
	meshComponent = static_cast<CMesh*>(mOwner->GetComponent(ComponentType::MESH));
	if (meshComponent != nullptr)
	{
		meshComponent->meshReference->meshShader.translation = translation;
		meshComponent->meshReference->meshShader.rotation = eulerRot;
		meshComponent->meshReference->meshShader.scale = scale;
	}
}

void CTransform::ReparentTransform(float4x4 matrix)
{
	matrix.Decompose(translation, rotation, scale);
	eulerRot = rotation.ToEulerXYZ();
	eulerRot *= RADTODEG;
	dirty_ = true;
}

void CTransform::UpdateBoundingBoxes()
{
	//if (mOwner->mesh != nullptr)
	//{
	//    gameObject->mesh->obb = gameObject->mesh->mesh->local_aabb;
	//    gameObject->mesh->obb.Transform(gameObject->transform->GetGlobalTransform());
	//    gameObject->mesh->global_aabb.SetNegativeInfinity();
	//    gameObject->mesh->global_aabb.Enclose(gameObject->mesh->obb);
	//}
}

void CTransform::UpdateTransformGuizmo(float4x4 matrix)
{
	mGlobalMatrix = matrix;
	if (mOwner->mParent->mTransform != nullptr)
	{
		float4x4 parentGlobal = mOwner->mParent->mTransform->mGlobalMatrix;
		parentGlobal.Inverse();

		mLocalMatrix = parentGlobal * mGlobalMatrix;
	}
	else
	{
		mLocalMatrix = mGlobalMatrix;
	}

	mLocalMatrix.Decompose(translation, rotation, scale);
	eulerRot = rotation.ToEulerXYZ();
	eulerRot *= RADTODEG;

	dirty_ = true;
}


