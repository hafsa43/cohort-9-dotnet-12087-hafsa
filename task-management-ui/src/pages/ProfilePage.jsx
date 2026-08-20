import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/services';

export default function ProfilePage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    authService.profile()
      .then(setProfile)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleLogout = () => { logout(); navigate('/login'); };

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">My Profile</h1>
          <p className="page-subtitle">Your account information</p>
        </div>
      </div>

      <div className="card" style={{ maxWidth: 480 }}>
        {/* Avatar */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '1.25rem', marginBottom: '1.5rem' }}>
          <div style={{
            width: 72, height: 72, borderRadius: '50%',
            background: 'linear-gradient(135deg, var(--accent), var(--accent-dark))',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            fontSize: '1.8rem', fontWeight: 800, color: '#fff', flexShrink: 0
          }}>
            {(profile?.firstName?.[0] || user?.email?.[0] || '?').toUpperCase()}
          </div>
          <div>
            <div style={{ fontSize: '1.2rem', fontWeight: 700 }}>{profile?.firstName} {profile?.lastName}</div>
            <div style={{ color: 'var(--text-muted)', fontSize: '0.875rem' }}>{profile?.email}</div>
            <span className={`badge ${profile?.role === 'Admin' ? 'badge-high' : 'badge-progress'}`} style={{ marginTop: '0.4rem' }}>
              {profile?.role}
            </span>
          </div>
        </div>

        <div className="divider" />

        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.85rem', marginBottom: '1.5rem' }}>
          <ProfileRow label="Email"      value={profile?.email} />
          <ProfileRow label="First Name" value={profile?.firstName} />
          <ProfileRow label="Last Name"  value={profile?.lastName} />
          <ProfileRow label="Role"       value={profile?.role} />
          <ProfileRow label="Member Since" value={profile?.createdAt ? new Date(profile.createdAt).toLocaleDateString() : '—'} />
        </div>

        <button id="logout-profile-btn" className="btn btn-danger" onClick={handleLogout} style={{ width: '100%', justifyContent: 'center' }}>
          🚪 Logout
        </button>
      </div>
    </div>
  );
}

function ProfileRow({ label, value }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
      <span style={{ color: 'var(--text-muted)', fontSize: '0.825rem', fontWeight: 600, textTransform: 'uppercase', letterSpacing: '0.05em' }}>{label}</span>
      <span style={{ color: 'var(--text-primary)', fontWeight: 500 }}>{value || '—'}</span>
    </div>
  );
}
