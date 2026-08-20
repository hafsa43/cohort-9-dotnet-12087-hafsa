import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { authService, userService } from '../services/services';

export default function ProfilePage() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [profile,   setProfile]   = useState(null);
  const [loading,   setLoading]   = useState(true);
  const [pwForm,    setPwForm]    = useState({ currentPassword: '', newPassword: '', confirm: '' });
  const [pwMsg,     setPwMsg]     = useState('');
  const [pwLoading, setPwLoading] = useState(false);

  useEffect(() => {
    authService.profile()
      .then(setProfile)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleLogout = () => { logout(); navigate('/login'); };

  const handlePwChange = async (e) => {
    e.preventDefault();
    if (pwForm.newPassword !== pwForm.confirm) {
      setPwMsg('❌ New passwords do not match.');
      return;
    }
    setPwLoading(true); setPwMsg('');
    try {
      await userService.changePassword({
        currentPassword: pwForm.currentPassword,
        newPassword:     pwForm.newPassword
      });
      setPwMsg('✅ Password changed successfully!');
      setPwForm({ currentPassword: '', newPassword: '', confirm: '' });
    } catch (err) {
      setPwMsg(`❌ ${err.response?.data?.message || 'Failed to change password.'}`);
    } finally {
      setPwLoading(false);
    }
  };

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">My Profile</h1>
          <p className="page-subtitle">Your account information</p>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', maxWidth: 900 }}>

        {/* ── Profile Card ─────────────────────────────────────────────────── */}
        <div className="card">
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
            <ProfileRow label="Email"        value={profile?.email} />
            <ProfileRow label="First Name"   value={profile?.firstName} />
            <ProfileRow label="Last Name"    value={profile?.lastName} />
            <ProfileRow label="Role"         value={profile?.role} />
            <ProfileRow label="Member Since" value={profile?.createdAt ? new Date(profile.createdAt).toLocaleDateString() : '—'} />
          </div>

          <button id="logout-profile-btn" className="btn btn-danger" onClick={handleLogout}
            style={{ width: '100%', justifyContent: 'center' }}>
            🚪 Logout
          </button>
        </div>

        {/* ── Change Password ───────────────────────────────────────────────── */}
        <div className="card">
          <h2 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: '1.25rem' }}>🔒 Change Password</h2>

          {pwMsg && (
            <div className={`alert ${pwMsg.includes('❌') ? 'alert-error' : 'alert-success'}`}>
              {pwMsg}
            </div>
          )}

          <form onSubmit={handlePwChange}>
            <div className="form-group">
              <label className="form-label">Current Password</label>
              <input id="current-password" className="form-input" type="password"
                placeholder="••••••••" required
                value={pwForm.currentPassword}
                onChange={e => setPwForm(f => ({ ...f, currentPassword: e.target.value }))} />
            </div>
            <div className="form-group">
              <label className="form-label">New Password</label>
              <input id="new-password" className="form-input" type="password"
                placeholder="Min 6 characters" required
                value={pwForm.newPassword}
                onChange={e => setPwForm(f => ({ ...f, newPassword: e.target.value }))} />
            </div>
            <div className="form-group">
              <label className="form-label">Confirm New Password</label>
              <input id="confirm-password" className="form-input" type="password"
                placeholder="Repeat new password" required
                value={pwForm.confirm}
                onChange={e => setPwForm(f => ({ ...f, confirm: e.target.value }))} />
            </div>
            <button id="change-pw-btn" className="btn btn-primary" type="submit" disabled={pwLoading}
              style={{ width: '100%', justifyContent: 'center' }}>
              {pwLoading ? 'Updating...' : '🔒 Update Password'}
            </button>
          </form>
        </div>

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
