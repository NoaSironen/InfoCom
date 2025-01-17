﻿using DataAccess.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataAccess.Repositories
{
    public class InvitationRepository
    {
        public static List<Invitation> Get(int id)
        {
            try
            {
                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    var invitations = session.Query<Invitation>()
                        .Where(x => x.User.Id == id && x.Meeting.Inactive == false)
                        .Fetch(x => x.Meeting)
                        .Fetch(x => x.User)
                        .ToList();
                    return invitations;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            return null;
        }

        public static List<Invitation> GetMeeting(int id)
        {
            try
            {
                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    var invitations = session.Query<Invitation>()
                        .Where(x => x.Meeting.Id == id)
                        .Fetch(x => x.Meeting)
                        .Fetch(x => x.User)
                        .ToList();
                    return invitations;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            return null;
        }


        public static void UpdateNotified(int id)
        {
            try
            {
                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    var invitations = session.Query<Invitation>()
                        .Where(x => x.User.Id == id && x.Notified == false)
                        .Fetch(x => x.Meeting)
                        .Fetch(x => x.User)
                        .ToList();
                    using (var transaction = session.BeginTransaction())
                    {
                        invitations.ForEach(x =>
                        {
                            x.Notified = true;
                        });
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static bool Add(Invitation invitation)
        {
            bool status = false;
            try
            {
                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    session.Save(invitation);
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            return status;
        }
        public static bool Update(Invitation invitation)
        {
            var success = false;

            try
            {

                using (var session = DbConnect.SessionFactory.OpenSession())
                {
                    var invt = session.Query<Invitation>().FirstOrDefault(x => x.Id == invitation.Id);
                    using (var transaction = session.BeginTransaction())
                    {
                        if (invt != null)
                        {
                            invt.Meeting = invitation.Meeting;
                            invt.Notified = invitation.Notified;
                            invt.Status = invitation.Status;
                            invt.User = invt.User;
                            session.Update(invt);
                        }
                        transaction.Commit();
                    }
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return success;
        }
    }
}
