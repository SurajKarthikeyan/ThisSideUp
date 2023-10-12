using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageStatus
{
    public int health = 10000;
    public int damage = 0;

    public enum Status { Waterlogged, Burnt, Zapped, None };

    private Status currentStatus;
    private float currentStatusTime = 0f;
    private float waterloggedTime = 10.0f;
    private float burnTime = 10.0f;
    private float zappedTime = 10.0f;
    private Package package;
    private int latestPackageApplyIndex = 0;
    private Coroutine fireDamage;

    public PackageStatus(Package p, int h, int d, float wt, float bt, float zt)
    {
        package = p;
        health = h;
        damage = d;
        currentStatus = Status.None;
        waterloggedTime = wt;
        burnTime = bt;
        zappedTime = zt;
    }

    public IEnumerator ApplyStatus(Status s)
    {
        if (currentStatus == s)     
            yield break; 
        Package.print("Applying status");
        // Status switch effects
        switch (s)
        {
            case Status.Zapped:
                if (currentStatus == Status.Waterlogged)
                    package.InflictZapDamage();
                break;
            case Status.Waterlogged:
                if (currentStatus == Status.Zapped)
                    package.InflictZapDamage();
                break;
        }
        //Remove current status
        RemoveStatus(latestPackageApplyIndex);
        currentStatus = s;
        latestPackageApplyIndex++;
        int packageApplyIndex = latestPackageApplyIndex;
        // End any ongoing fire damage
        if (fireDamage != null) package.StopCoroutine(fireDamage);
        // Start timer before the status is removed
        switch (s)
        {
            case Status.Burnt:
                fireDamage = package.InflictBurnDamage();
                package.currentMaterial = package.fireMaterial;
                package.render.material = package.fireMaterial;
                package.fireEffects.SetActive(true);
                yield return new WaitForSeconds(burnTime);
                break;
            case Status.Waterlogged:
                package.currentMaterial = package.wetMaterial;
                package.render.material = package.wetMaterial;
                package.waterEffects.SetActive(true);
                yield return new WaitForSeconds(waterloggedTime);
                break;
            case Status.Zapped:
                package.InflictZapDamage();
                package.currentMaterial = package.electrocutedMaterial;
                package.render.material = package.electrocutedMaterial;
                package.electricEffects.SetActive(true);
                yield return new WaitForSeconds(zappedTime);
                break;
        }
        RemoveStatus(packageApplyIndex);
    }

    public void RemoveStatus(int packageApplyIndex)
    {
        if (packageApplyIndex == latestPackageApplyIndex && package != null) 
        {
            package.currentMaterial = package.defaultMaterial;
            package.render.material = package.defaultMaterial;
            package.fireEffects.SetActive(false);
            package.waterEffects.SetActive(false);
            package.electricEffects.SetActive(false);
            currentStatus = Status.None; 
        }
    }

    public Status GetCurrentStatus()
    {
        return currentStatus;
    }

    public bool IsWaterlogged()
    {
        return currentStatus == Status.Waterlogged;
    }

    public bool IsBurnt()
    {
        return currentStatus == Status.Burnt;
    }

    public bool IsZapped()
    {
        return currentStatus == Status.Zapped;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        this.damage += damage;
        if (ScoreManager.S != null)
            ScoreManager.S.SetDamage(this.damage);
    }

}

