namespace YadnexTank.PhantomAmmo.AspNetCore
{
	public class PhantomAmmoCollectorOptions
    {
	    public bool Enabled { get; set; } = false;
	    
	    public string AllRequestsFile { get; set; } = "ammoAll.txt";
	    
	    public string GoodRequestsFile { get; set; } = "ammoGood.txt";
	    
	    public string BadRequestsFile { get; set; } = "ammoBad.txt"; 
    }
}