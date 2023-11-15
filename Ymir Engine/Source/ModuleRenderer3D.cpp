#include "ModuleRenderer3D.h"

#include "Application.h"
#include "ModuleWindow.h"
#include "ModuleCamera3D.h"
#include "ModuleEditor.h"
#include "ModuleInput.h"
#include "ModuleScene.h"
#include "Globals.h"
#include "Log.h"
#include "GameObject.h"

#include "External/Optick/include/optick.h"

ModuleRenderer3D::ModuleRenderer3D(Application* app, bool start_enabled) : Module(app, start_enabled)
{
	context = nullptr;

	FBO = 0;
	TCB = 0;
	RBO = 0;

	LOG("Creating ModuleRenderer3D");

}

// Destructor
ModuleRenderer3D::~ModuleRenderer3D()
{

}

// Called before render is available
bool ModuleRenderer3D::Init()
{
	LOG("Creating 3D Renderer context");
	bool ret = true;

	// Stream Assimp Log messages to Debug window
	EnableAssimpDebugger();

	// OpenGL initial attributes
	SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE); // Using OpenGL core profile
	SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
	SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
	SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);
	
	// Create context
	context = SDL_GL_CreateContext(App->window->window);
	if(context == NULL)
	{
		LOG("OpenGL context could not be created! SDL_Error: %s\n", SDL_GetError());
		ret = false;
	}
	else 
	{
		LOG("OpenGL context created sucessfully!");
	}

	// Initializing Glew
	GLenum err = glewInit();
	if (err != GLEW_OK) {
		LOG("Error in loading Glew: %s\n", glewGetErrorString(err));
	}
	else {
		LOG("Successfully using Glew %s", glewGetString(GLEW_VERSION));
	}

	// Initializing DevIL
	ilInit();
	ILenum ILerror = ilGetError();
	if (ILerror != IL_NO_ERROR) {
		LOG("Error in loading DevIL: %s\n", iluErrorString(ILerror));
	}
	else {
		LOG("Successfully using DevIL %d", iluGetInteger(IL_VERSION_NUM));
	}
	
	if(ret == true)
	{
		// Use Vsync
		if(VSYNC && SDL_GL_SetSwapInterval(1) < 0)
			LOG("Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());

		// Initialize Projection Matrix
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		// Check for errors
		GLenum error = glGetError();
		if(error != GL_NO_ERROR)
		{
			LOG("Error initializing OpenGL! %s\n", gluErrorString(error));
			ret = false;
		}

		// Initialize Modelview Matrix
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();
		
		// Check for errors
		error = glGetError();
		if(error != GL_NO_ERROR)
		{
			LOG("Error initializing OpenGL! %s\n", gluErrorString(error));
			ret = false;
		}
		
		glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);
		glClearDepth(1.0f);
		
		//Initialize clear color
		glClearColor(0.f, 0.f, 0.f, 1.f);

		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		//Check for error
		error = glGetError();
		if(error != GL_NO_ERROR)
		{
			LOG("Error initializing OpenGL! %s\n", gluErrorString(error));
			ret = false;
		}
		
		GLfloat LightModelAmbient[] = {0.0f, 0.0f, 0.0f, 1.0f};
		glLightModelfv(GL_LIGHT_MODEL_AMBIENT, LightModelAmbient);
		
		lights[0].ref = GL_LIGHT0;
		lights[0].ambient.Set(0.25f, 0.25f, 0.25f, 1.0f);
		lights[0].diffuse.Set(0.75f, 0.75f, 0.75f, 1.0f);
		lights[0].SetPos(0.0f, 0.0f, 2.5f);
		lights[0].Init();
		
		GLfloat MaterialAmbient[] = {1.0f, 1.0f, 1.0f, 1.0f};
		glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, MaterialAmbient);

		GLfloat MaterialDiffuse[] = {1.0f, 1.0f, 1.0f, 1.0f};
		glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, MaterialDiffuse);
		
		// Enable OpenGL initial configurations

		glEnable(GL_DEPTH_TEST);
		glEnable(GL_CULL_FACE);
		lights[0].Active(true);
		glEnable(GL_LIGHTING);
		glEnable(GL_COLOR_MATERIAL);
		glEnable(GL_TEXTURE_2D);

		// Additional OpenGL configurations (starting disabled)

		glDisable(GL_TEXTURE_3D);
		glDisable(GL_BLEND);
		glDisable(GL_MULTISAMPLE);
		glDisable(GL_STENCIL_TEST);
		glDisable(GL_SCISSOR_TEST);
		glDisable(GL_ALPHA_TEST);
		glDisable(GL_POINT_SPRITE);
		glDisable(GL_FOG);
		glDisable(GL_POINT_SMOOTH);
		glDisable(GL_LINE_SMOOTH);
		glDisable(GL_NORMALIZE);
		glDisable(GL_POLYGON_OFFSET_FILL);

		// Wireframe-Only Mode starting disabled

		glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);

	}

	// Projection matrix for
	OnResize(SCREEN_WIDTH, SCREEN_HEIGHT);

	LOG("OpenGL initialized successfully.");

	Grid.axis = true;

	// Load Baker House from the start

	models.push_back(Model("Assets/BakerHouse.fbx"));

	// Generate a Framebuffer Object (FBO)

	glGenFramebuffers(1, &FBO);
	glBindFramebuffer(GL_FRAMEBUFFER, FBO);

	// Create a Texture Attachment (Texture Color Buffer [TCB])

	glGenTextures(1, &TCB);
	glBindTexture(GL_TEXTURE_2D, TCB);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, SCREEN_WIDTH, SCREEN_HEIGHT, 0, GL_RGB, GL_UNSIGNED_BYTE, nullptr);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, TCB, 0);

	// Create a Renderbuffer Attachment

	glGenRenderbuffers(1, &RBO);
	glBindRenderbuffer(GL_RENDERBUFFER, RBO);
	glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, SCREEN_WIDTH, SCREEN_HEIGHT);

	glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, RBO);

	// Check Framebuffer Completeness
	
	if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE) {

		LOG("Framebuffer is not complete");

	}

	// Bind the Default Framebuffer

	glBindFramebuffer(GL_FRAMEBUFFER, 0);

	return ret;
}

// PreUpdate: clear buffer
update_status ModuleRenderer3D::PreUpdate(float dt)
{
	OPTICK_EVENT();

	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glLoadIdentity();

	glMatrixMode(GL_MODELVIEW);
	glLoadMatrixf(App->camera->GetViewMatrix());

	// light 0 on cam pos
	lights[0].SetPos(App->camera->Position.x, App->camera->Position.y, App->camera->Position.z);

	for(uint i = 0; i < MAX_LIGHTS; ++i)
		lights[i].Render();

	App->editor->AddFPS(App->GetFPS());
	App->editor->AddDT(App->GetDT());
	App->editor->AddMS(App->GetMS());
	
	return UPDATE_CONTINUE;
}

// PostUpdate present buffer to screen
update_status ModuleRenderer3D::PostUpdate(float dt)
{
	OPTICK_EVENT();

	// Your rendering code here
	glBindFramebuffer(GL_FRAMEBUFFER, FBO);
	glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	
	// Render Grid
	
	if (showGrid) {

		Grid.Render();

	}

	// -------------------- Drawing 3D Models ----------------------

	HandleDragAndDrop();

	DrawModels();

	// --------------------------- Drawing editor and Swaping Window -------------------------

	// Framebuffer Render

	
	//glViewport(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT); // Set the viewport size to match the framebuffer size
	//glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
	//glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
	//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT); // Clear color and depth buffers

	// Your additional rendering code for the framebuffer goes here
	
	glBindFramebuffer(GL_FRAMEBUFFER, 0); // Switch back to the default framebuffer
	glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	App->editor->DrawEditor();
	SDL_GL_SwapWindow(App->window->window);
	
	return UPDATE_CONTINUE;
}

// Called before quitting
bool ModuleRenderer3D::CleanUp()
{
	LOG("Destroying 3D Renderer");

	// Clean Framebuffer
	glDeleteFramebuffers(1, &FBO);
	glDeleteTextures(1, &TCB);

	// Detach Assimp Log Stream
	CleanUpAssimpDebugger();

	// Shutdown DevIL
	ilShutDown();
	
	// Delete OpenGL context
	SDL_GL_DeleteContext(context);

	return true;
}


void ModuleRenderer3D::OnResize(int width, int height)
{
	glViewport(0, 0, width, height);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();

	//todo: USE MATHGEOLIB here BEFORE 1st delivery! (TIP: Use MathGeoLib/Geometry/Frustum.h, view and projection matrices are managed internally.)
	ProjectionMatrix = perspective(60.0f, (float)width / (float)height, 0.125f, 512.0f);
	glLoadMatrixf(ProjectionMatrix.M);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
}

void ModuleRenderer3D::HandleDragAndDrop()
{
	if (App->input->droppedFile) {

		if (IsFileExtension(App->input->droppedFileDirectory, ".fbx") || IsFileExtension(App->input->droppedFileDirectory, ".FBX")) {

			models.push_back(Model(App->input->droppedFileDirectory));

			ReloadTextures();

		}
		else if (IsFileExtension(App->input->droppedFileDirectory, ".png") || IsFileExtension(App->input->droppedFileDirectory, ".dds")) {

			ClearActualTexture();

			for (auto it = models.begin(); it != models.end(); ++it) {

				for (auto jt = (*it).meshes.begin(); jt != (*it).meshes.end(); ++jt) {

					if ((*jt).meshGO->selected || (*it).modelGO->selected) {

						(*jt).loadedTextures = false;

						Texture tmpTexture;

						tmpTexture.path = App->input->droppedFileDirectory;

						(*jt).textures.push_back(tmpTexture);

					}

				}

			}

		}
		
		App->input->droppedFile = false;

	}
}

bool ModuleRenderer3D::IsFileExtension(const char* directory, const char* extension)
{
	size_t strLen = strlen(directory);
	size_t suffixLen = strlen(extension);

	if (strLen < suffixLen) {

		return false;
	}

	return strncmp(directory + strLen - suffixLen, extension, suffixLen) == 0;
}

void ModuleRenderer3D::ApplyCheckerTexture()
{
	ClearActualTexture();

	for (auto it = models.begin(); it != models.end(); ++it) {

		for (auto jt = (*it).meshes.begin(); jt != (*it).meshes.end(); ++jt) {

			if ((*jt).meshGO->selected || (*it).modelGO->selected) {

				(*jt).loadedTextures = false;

				Texture checkerTexture;

				(*jt).textures.push_back(checkerTexture);

				(*jt).applyCheckerTexture = true;

			}

		}

	}

}

void ModuleRenderer3D::ClearActualTexture()
{
	for (auto it = models.begin(); it != models.end(); ++it) {

		for (auto jt = (*it).meshes.begin(); jt != (*it).meshes.end(); ++jt) {

			if ((*jt).meshGO->selected || (*it).modelGO->selected) {

				(*jt).textures.clear();

			}
			
		}

	}

}

void ModuleRenderer3D::ReloadTextures()
{
	for (auto it = models.begin(); it != models.end(); ++it) {

		for (auto jt = (*it).meshes.begin(); jt != (*it).meshes.end(); ++jt) {

			(*jt).loadedTextures = false;

		}

	}
}

void ModuleRenderer3D::DrawBoundingBox(float3* vertices, float3 color)
{
	uint indices[24] = { 

		0,2,2,
		6,6,4,
		4,0,0,
		1,1,3,
		3,2,4,
		5,6,7,
		5,7,3,
		7,1,5 

	};

	glBegin(GL_LINES);
	glColor3fv(color.ptr());

	for (size_t i = 0; i < (sizeof(indices) / sizeof(indices[0])); i++) {

		glVertex3fv(vertices[indices[i]].ptr());

	}

	glColor3f(255.f, 255.f, 255.f);
	glEnd();
}

void ModuleRenderer3D::DrawFrustumPlane(Plane plane)
{
	
}

void ModuleRenderer3D::DrawModels()
{
	for (auto it = models.begin(); it != models.end(); ++it) {

		(*it).DrawModel();

	}
}

void ModuleRenderer3D::ClearModels()
{
	models.clear();
}

void ModuleRenderer3D::EnableAssimpDebugger()
{
	struct aiLogStream stream;
	stream = aiGetPredefinedLogStream(aiDefaultLogStream_DEBUGGER, nullptr);
	aiAttachLogStream(&stream);
}

void ModuleRenderer3D::CleanUpAssimpDebugger()
{
	aiDetachAllLogStreams();
}
