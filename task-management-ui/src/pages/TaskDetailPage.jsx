import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { taskService } from '../services/services';

const Badge = ({ value, map }) => <span className={`badge ${map[value] || ''}`}>{value}</span>;

export default function TaskDetailPage() {
  const { id } = useParams();
  const [task, setTask] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    taskService.getById(id)
      .then(setTask)
      .catch(() => setError('Task not found or access denied.'))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <div className="spinner" />;
  if (error)   return <div className="alert alert-error">{error}</div>;
  if (!task)   return null;

  const statusMap   = { Pending: 'badge-pending', InProgress: 'badge-progress', Completed: 'badge-done' };
  const priorityMap = { High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low' };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Task #{task.id}</h1>
          <p className="page-subtitle">Task Details</p>
        </div>
        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <Link to="/tasks" className="btn btn-ghost">← Back</Link>
          <Link to={`/tasks/${task.id}/edit`} className="btn btn-primary" id="edit-task-btn">✏️ Edit Task</Link>
        </div>
      </div>

      <div className="card" style={{ maxWidth: 720 }}>
        <h2 style={{ fontSize: '1.4rem', fontWeight: 700, marginBottom: '1rem' }}>{task.title}</h2>
        <div className="divider" />

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
          <Detail label="Status"      value={<Badge value={task.status}   map={statusMap} />} />
          <Detail label="Priority"    value={<Badge value={task.priority} map={priorityMap} />} />
          <Detail label="Assigned To" value={task.assignedToUserName || '—'} />
          <Detail label="Category"    value={task.categoryName || '—'} />
          <Detail label="Due Date"    value={task.dueDate ? new Date(task.dueDate).toLocaleDateString() : '—'} />
          <Detail label="Created"     value={new Date(task.createdAt).toLocaleString()} />
        </div>

        {task.description && (
          <>
            <div className="divider" />
            <div>
              <div className="form-label" style={{ marginBottom: '0.5rem' }}>Description</div>
              <p style={{ color: 'var(--text-secondary)', lineHeight: 1.8 }}>{task.description}</p>
            </div>
          </>
        )}
      </div>
    </div>
  );
}

function Detail({ label, value }) {
  return (
    <div>
      <div className="form-label" style={{ marginBottom: '0.3rem' }}>{label}</div>
      <div style={{ color: 'var(--text-primary)', fontWeight: 500 }}>{value}</div>
    </div>
  );
}
