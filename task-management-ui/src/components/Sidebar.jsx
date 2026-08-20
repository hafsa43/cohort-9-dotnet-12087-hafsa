import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const links = [
  { to: '/dashboard', label: 'Dashboard',   icon: '📊' },
  { to: '/tasks',     label: 'Tasks',        icon: '✅' },
  { to: '/tasks/new', label: 'New Task',     icon: '➕' },
  { to: '/profile',   label: 'My Profile',   icon: '👤' },
];

export default function Sidebar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => { logout(); navigate('/login'); };

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
      <div className="sidebar-bottom">
        <div style={{ padding: '0.5rem 0.75rem', fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
          {user?.fullName || user?.email}
          <div style={{ color: 'var(--accent-light)', fontWeight: 600 }}>{user?.role}</div>
        </div>
        <button className="nav-link" onClick={handleLogout} id="logout-btn">
          <span className="nav-icon">🚪</span> Logout
        </button>
      </div>
    </nav>
  );
}
