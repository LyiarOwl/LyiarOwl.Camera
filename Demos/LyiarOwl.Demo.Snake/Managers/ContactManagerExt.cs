using System;
using Genbox.VelcroPhysics.Collision.ContactSystem;

namespace LyiarOwl.Demo.Snake.Managers;

public static class ContactManagerExt
{
    public static void FindNewContacts()
    {
        if (WorldManager.Instance == null)
        {
            throw new NullReferenceException($"[{typeof(ContactManagerExt).Name} :: {nameof(FindNewContacts)}]: initialize the world manager!");
        }

        WorldManager.Instance.World.ContactManager.BroadPhase.UpdatePairs(WorldManager.Instance.World.ContactManager.OnBroadphaseCollision);
    }
}