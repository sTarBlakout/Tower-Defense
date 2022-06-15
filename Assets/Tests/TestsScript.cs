using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TowerDefence.Enemy;
using TowerDefence.Hero;
using TowerDefence.Player;
using UnityEngine;
using UnityEngine.TestTools;

public class TestsScript
{
    [Test]
    public void TestEnemyDeathMoney()
    {
        var enemy = new GameObject().AddComponent<EnemyBehavior>();
        Assert.IsTrue(enemy.GetMoneyForKill() == 20);
    }
    
    [Test]
    public void TestEnemyDamage()
    {
        var enemy = new GameObject().AddComponent<EnemyBehavior>();
        enemy.currHealth = 100f;
        enemy.TakeDamage(50f);
        Assert.IsTrue(enemy.currHealth == 50f);
    }
    
    [Test]
    public void TestBaseDestroyed()
    {
        var mainBase = new GameObject().AddComponent<PlayerVillageBehavior>();
        mainBase.TakeDamage(100f);
        Assert.IsTrue(mainBase.IsDead());
    }
    
    [Test]
    public void TestHeroFireRate()
    {
        var hero = new GameObject().AddComponent<HeroBehavior>();
        Assert.IsFalse(hero.IsReadyToShoot());
    }
    
    [Test]
    public void TestTowerSetHero()
    {
        var hero = new GameObject();
        var tower = new GameObject().AddComponent<TowersBehavior>();
        tower.SetNewHero(hero);
        Assert.IsTrue(tower.IsBusy());
    }
    
    [Test]
    public void TestBaseTakeDamage()
    {
        var mainBase = new GameObject().AddComponent<PlayerVillageBehavior>();
        mainBase.TakeDamage(50f);
        Assert.IsTrue(mainBase.currentHealth == 50f);
    }
}
