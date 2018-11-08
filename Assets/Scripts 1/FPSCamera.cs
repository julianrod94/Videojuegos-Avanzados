using System;
using UnityEngine;

[Serializable]
public class FPSCamera : MonoBehaviour {
    
	public float sensibilidadX = 2f;
    public float sensibilidadY = 2f;
	public bool bloquearCursor = true;

	// El parametro Header nos permite escribir texto en el editor de unity
	[Header ("Opciones Clamp: ")]
	public bool clampVertical = true;
    public float minX = -90f;
    public float maxX = 70f;

	private Quaternion nuevaRotacionPlayer;
	private Quaternion nuevaRotacionCamara;
	private bool cursorBloqueado = true;
	private Camera camara;
    
	void Start()  {
		// Obtenemos el coponente de Camara que se encuentra como hijo 
		camara = GetComponentInChildren<Camera> ();
      	// Guardamos la  rotacion original local del player
		nuevaRotacionPlayer = transform.localRotation;
		// Guardamos la rotacion original local de la camara
        nuevaRotacionCamara = camara.transform.localRotation;
    }


	void Update(){
		LookRotation ();
	}

    public void LookRotation() {
		// Obtenemos los ejes del Mouse y los multiplicamos por la sensibilidad
		float yRot = Input.GetAxis("Mouse X") * sensibilidadX;
        float xRot = Input.GetAxis("Mouse Y") * sensibilidadY;

		// A la rotacion inicial la multiplicamos por los ejes X e Y obtenidos previamente
        nuevaRotacionPlayer *= Quaternion.Euler (0f, yRot, 0f);
        nuevaRotacionCamara *= Quaternion.Euler (-xRot, 0f, 0f);

		// Si el clamp vertical esta activo, llamamos a la funcion que evita que el angulo supere el numero determinado
		if (clampVertical == true) {
			nuevaRotacionCamara = ClampRotacionVertical (nuevaRotacionCamara);
		}
       
		// Una vez listos los cálculos de rotacion, asignamos esa rotacion a los objetos
        transform.localRotation = nuevaRotacionPlayer;
	    if (camara != null) {
		    camara.transform.localRotation = nuevaRotacionCamara;
	    }

	    // Checkeamos el estado de bloque del cursor
		if (bloquearCursor == true ) DetectarBloqueoDelMouse();
    }

    private void DetectarBloqueoDelMouse() {
        
		if(Input.GetKeyUp(KeyCode.Escape)) {
            cursorBloqueado = false;
        }
        else if(Input.GetMouseButtonUp(0)) {
            cursorBloqueado = true;
        }

        if (cursorBloqueado == true) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
		else if (cursorBloqueado == false) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotacionVertical(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

        angleX = Mathf.Clamp (angleX, minX, maxX);

        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

}

