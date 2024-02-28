#pragma once

#include "Component.h"
#include <vector>

class btRigidBody;

class CRigidBody : public Component
{
public:
	CRigidBody(GameObject* owner);
	~CRigidBody();

	void Update();
	void OnInspector();

	//TODO: funcions per canviar els valors del Cos R�gid

private:
	btRigidBody* body;

	//TODO: crear variables (massa, usegravity, etc.)
};