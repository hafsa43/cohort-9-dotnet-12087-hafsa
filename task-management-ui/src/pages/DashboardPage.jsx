import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { taskService } from '../services/services';

export default function DashboardPage() {
  const { user, isAdmin } = useAuth();
  const [counts, setCounts] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    taskService.getCounts()
      .then(setCounts)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Dashboard 👋</h1>
          <p className="page-subtitle">
            Welcome back, <span className="text-accent">{user?.fullName || user?.email}</span>
            {isAdmin ? ' — viewing all tasks (Admin)' : ' — showing your tasks'}
          </p>
        </div>
      </div>

      <div className="stat-grid">
        <div className="stat-card pending">
          <div className="stat-icon">🕐</div>
          <div className="stat-value" style={{ color: '#f59e0b' }}>{counts?.pending ?? 0}</div>
          <div className="stat-label">Pending</div>
        </div>
        <div className="stat-card progress">
          <div className="stat-icon">🔄</div>
          <div className="stat-value" style={{ color: '#3b82f6' }}>{counts?.inProgress ?? 0}</div>
          <div className="stat-label">In Progress</div>
        </div>
        <div className="stat-card done">
          <div className="stat-icon">✅</div>
          <div className="stat-value" style={{ color: '#22c55e' }}>{counts?.completed ?? 0}</div>
          <div className="stat-label">Completed</div>
        </div>
        <div className="stat-card total">
          <div className="stat-icon">📊</div>
          <div className="stat-value" style={{ color: '#818cf8' }}>{counts?.total ?? 0}</div>
          <div className="stat-label">Total Tasks</div>
        </div>
      </div>

      <div className="card">
        <h2 style={{ fontSize: '1rem', fontWeight: 700, marginBottom: '0.5rem' }}>Quick Tips</h2>
        <ul style={{ color: 'var(--text-secondary)', fontSize: '0.875rem', paddingLeft: '1.25rem', display: 'flex', flexDirection: 'column', gap: '0.4rem' }}>
          <li>Use <strong style={{ color: 'var(--accent-light)' }}>Tasks → New Task</strong> to add a new task</li>
          <li>Click any task row to view its details</li>
          {isAdmin && <li>As Admin, you can see and manage all users' tasks</li>}
        </ul>
      </div>
    </div>
  );
}
