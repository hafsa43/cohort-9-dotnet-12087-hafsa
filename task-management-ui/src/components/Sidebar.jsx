import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const baseLinks = [
  { to: '/dashboard', label: 'Dashboard', icon: '📊' },
  { to: '/tasks',     label: 'Tasks',     icon: '✅' },
  { to: '/tasks/new', label: 'New Task',  icon: '➕' },
  { to: '/profile',   label: 'My Profile', icon: '👤' },
];

const adminLinks = [
  { to: '/admin/users',      label: 'Users',      icon: '👥' },
];

export default function Sidebar() {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => { logout(); navigate('/login'); };

  const links = isAdmin ? [...baseLinks, ...adminLinks] : baseLinks;

  return (
    <nav className="sidebar">
      <div className="sidebar-logo">⚡ TaskFlow</div>

      {links.map(l => (
        <NavLink
          key={l.to}
          to={l.to}
          className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
        >
          <span className="nav-icon">{l.icon}</span>
          {l.label}
        </NavLink>
      ))}

      {isAdmin && (
        <div style={{ fontSize: '0.7rem', color: 'var(--text-muted)', padding: '0.4rem 0.75rem', marginTop: '0.5rem', textTransform: 'uppercase', letterSpacing: '0.08em' }}>
          Admin Panel
        </div>
      )}

      <div className="sidebar-bottom">
        <div style={{ padding: '0.5rem 0.75rem', fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
          <div style={{ color: 'var(--text-primary)', fontWeight: 600 }}>
            {user?.fullName || user?.email}
          </div>
          <span className={`badge ${user?.role === 'Admin' ? 'badge-high' : 'badge-progress'}`}
            style={{ marginTop: '0.3rem' }}>
            {user?.role}
          </span>
        </div>
        <button className="nav-link" onClick={handleLogout} id="logout-btn">
          <span className="nav-icon">🚪</span> Logout
        </button>
      </div>
    </nav>
  );
}
