namespace YadnexTank.PhantomAmmo.AspNetCore
{
	public class PhantomAmmoCollectorOptions
    {
	    public bool Enabled { get; set; } = false;
	    
	    public string PathToFile { get; set; } = "ammo.txt";
    }
}