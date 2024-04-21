# CREATE POOL

```
 UniPool<Projectile> pool;
 private void Start()
 {
     pool = new UniPool<Projectile>(projectilePrefab, 10, 10);        
 }
```

# USE POOL

```
private void Fire()
{        
    Projectile projectile = pool.Get();
    projectile.damage = damage;
    projectile.transform.SetPositionAndRotation(muzzle.position, muzzle.rotation);
}
```
