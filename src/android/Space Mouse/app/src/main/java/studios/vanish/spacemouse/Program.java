package studios.vanish.spacemouse;
import android.content.Context;
import android.graphics.Point;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.support.constraint.ConstraintLayout;
import android.support.v4.view.MotionEventCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Display;
import android.view.MotionEvent;
import android.view.View;
import android.webkit.WebView;
import android.widget.EditText;
import android.widget.TextView;
public class Program extends AppCompatActivity
{
	SensorManager DefaultSensorManager;
	Sensor GyroSensor;
	TextView design_label;
	ConstraintLayout design_root;
	double x;
	double y;
	double z;
	double px;
	double py;
	double pz;
	boolean left;
	boolean right;
	boolean scroll;
	int displayWidth;
	String IP = "";
	WebView design_web;
	SensorEventListener GyroSensorListener = new SensorEventListener()
	{
		public void onSensorChanged(SensorEvent event)
		{
			float xAxis = event.values[0];
			float yAxis = event.values[1];
			float zAxis = event.values[2];
			x = (int)(xAxis * 1) / 1.0;
			y = (int)(yAxis * 1) / 1.0;
			z = (int)(zAxis * 1) / 1.0;
			design_label.setText("(" + x + ", " + y + ", " + z + ")");
			SendInformation();
			px = x;
			py = y;
			pz = z;
		}
		public void onAccuracyChanged(Sensor sensor, int accuracy)
		{

		}
	};
	public void SetIP(View view)
	{
		IP = ((EditText)findViewById(R.id.design_text)).getText().toString();
	}
	public void SendInformation()
	{
		design_web.loadUrl("http://" + IP + "/x:" + x + "?y:" + y + "?z:" + z + "?left:" + left + "?right:" + right + "?scroll:" + scroll);
	}
	protected void onCreate(Bundle savedInstanceState)
	{
		super.onCreate(savedInstanceState);
		setContentView(R.layout.design);
		InitializeComponents();
	}
	public void InitializeComponents()
	{
		Display display = getWindowManager().getDefaultDisplay();
		Point point = new Point();
		display.getSize(point);

		displayWidth = point.x;
		DefaultSensorManager = (SensorManager)getSystemService(Context.SENSOR_SERVICE);
		GyroSensor = DefaultSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
		DefaultSensorManager.registerListener(GyroSensorListener, GyroSensor, SensorManager.SENSOR_DELAY_FASTEST);
		design_label = (TextView)findViewById(R.id.design_label);
		design_web = new WebView(getApplicationContext());
		design_root = (ConstraintLayout)findViewById(R.id.design_root);
		design_root.setOnTouchListener(new View.OnTouchListener()
		{
			@Override
			public boolean onTouch(View v, MotionEvent event)
			{
				int action = MotionEventCompat.getActionMasked(event);
				boolean _return = false;
				double xPos = event.getX();
				double yPos = event.getY();
				switch(action)
				{
					case MotionEvent.ACTION_DOWN:
						if (xPos < (displayWidth / 2))
						{
							left = true;
						}
						else if (xPos > (displayWidth / 2))
						{
							right = true;
						}
						SendInformation();
						_return = true;
						break;
					case MotionEvent.ACTION_UP:
						if (xPos < (displayWidth / 2))
						{
							left = false;
						}
						else if (xPos > (displayWidth / 2))
						{
							right = false;
						}
						SendInformation();
						_return = true;
						break;
					default:
						_return = false;
						break;
				}
				return _return;
			}
		});
	}
}
